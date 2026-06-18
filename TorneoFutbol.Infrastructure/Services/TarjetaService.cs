using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Tarjetas;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class TarjetaService : ITarjetaService
{
    private readonly TorneoFutbolDbContext _context;

    public TarjetaService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Helper mapeo
    private static TarjetaResponse MapToResponse(Tarjeta t) => new()
    {
        Id = t.Id,
        PartidoId = t.PartidoId,
        JugadorId = t.JugadorId,
        NombreJugador = t.Jugador.Nombre,
        EquipoId = t.EquipoId,
        NombreEquipo = t.Equipo.Nombre,
        Tipo = t.Tipo,
        Minuto = t.Minuto
    };

    // Obtener tarjetas de un partido
    public async Task<IEnumerable<TarjetaResponse>> GetByPartidoAsync(int partidoId)
    {
        var tarjetas = await _context.Tarjetas
            .Include(t => t.Jugador)
            .Include(t => t.Equipo)
            .Where(t => t.PartidoId == partidoId)
            .OrderBy(t => t.Minuto)
            .ToListAsync();

        return tarjetas.Select(MapToResponse);
    }

    // Registrar tarjeta
    public async Task<TarjetaResponse> RegistrarAsync(int partidoId, RegistrarTarjetaRequest request, int organizadorId)
    {
        var partido = await _context.Partidos
            .Include(p => p.Torneo)
            .FirstOrDefaultAsync(p => p.Id == partidoId);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para registrar tarjetas en este partido.");

        if (partido.Estado == "Pendiente" || partido.Estado == "Suspendido")
            throw new Exception("No se pueden registrar tarjetas en un partido pendiente o suspendido.");

        if (request.Tipo != "Amarilla" && request.Tipo != "Roja")
            throw new Exception("El tipo de tarjeta debe ser 'Amarilla' o 'Roja'.");

        if (request.Minuto < 1 || request.Minuto > 120)
            throw new Exception("El minuto debe estar entre 1 y 120.");

        // Verificar que el jugador pertenece al equipo
        var jugador = await _context.Jugadores
            .FirstOrDefaultAsync(j => j.Id == request.JugadorId && j.EquipoId == request.EquipoId);

        if (jugador is null)
            throw new Exception("El jugador no pertenece al equipo indicado.");

        if (!jugador.Activo)
            throw new Exception("El jugador está dado de baja.");

        // Verificar que el equipo participa en el partido
        if (partido.EquipoLocalId != request.EquipoId && partido.EquipoVisitanteId != request.EquipoId)
            throw new Exception("El equipo no participa en este partido.");

        // Verificar si el jugador ya tiene roja en este partido
        var tieneRoja = await _context.Tarjetas
            .AnyAsync(t => t.PartidoId == partidoId &&
                          t.JugadorId == request.JugadorId &&
                          t.Tipo == "Roja");

        if (tieneRoja)
            throw new Exception("El jugador ya tiene tarjeta roja en este partido.");

        // Verificar doble amarilla
        if (request.Tipo == "Amarilla")
        {
            var amarillas = await _context.Tarjetas
                .CountAsync(t => t.PartidoId == partidoId &&
                                t.JugadorId == request.JugadorId &&
                                t.Tipo == "Amarilla");

            if (amarillas >= 2)
                throw new Exception("El jugador ya tiene dos amarillas en este partido.");
        }

        var tarjeta = new Tarjeta
        {
            PartidoId = partidoId,
            JugadorId = request.JugadorId,
            EquipoId = request.EquipoId,
            Tipo = request.Tipo,
            Minuto = request.Minuto
        };

        _context.Tarjetas.Add(tarjeta);
        await _context.SaveChangesAsync();

        await _context.Entry(tarjeta).Reference(t => t.Jugador).LoadAsync();
        await _context.Entry(tarjeta).Reference(t => t.Equipo).LoadAsync();

        return MapToResponse(tarjeta);
    }

    // Anular tarjeta
    public async Task AnularAsync(int tarjetaId, int organizadorId)
    {
        var tarjeta = await _context.Tarjetas
            .Include(t => t.Partido)
                .ThenInclude(p => p.Torneo)
            .FirstOrDefaultAsync(t => t.Id == tarjetaId);

        if (tarjeta is null)
            throw new Exception("Tarjeta no encontrada.");

        if (tarjeta.Partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para anular esta tarjeta.");

        _context.Tarjetas.Remove(tarjeta);
        await _context.SaveChangesAsync();
    }
}