using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Goles;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class GolService : IGolService
{
    private readonly TorneoFutbolDbContext _context;

    public GolService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Helper mapeo
    private static GolResponse MapToResponse(Gol g) => new()
    {
        Id = g.Id,
        PartidoId = g.PartidoId,
        JugadorId = g.JugadorId,
        NombreJugador = g.Jugador.Nombre,
        EquipoId = g.EquipoId,
        NombreEquipo = g.Equipo.Nombre,
        Minuto = g.Minuto,
        EsAutogol = g.EsAutogol
    };

    // Obtener goles de un partido
    public async Task<IEnumerable<GolResponse>> GetByPartidoAsync(int partidoId)
    {
        var goles = await _context.Goles
            .Include(g => g.Jugador)
            .Include(g => g.Equipo)
            .Where(g => g.PartidoId == partidoId)
            .OrderBy(g => g.Minuto)
            .ToListAsync();

        return goles.Select(MapToResponse);
    }

    // Registrar gol
    public async Task<GolResponse> RegistrarAsync(int partidoId, RegistrarGolRequest request, int organizadorId)
    {
        var partido = await _context.Partidos
            .Include(p => p.Torneo)
            .FirstOrDefaultAsync(p => p.Id == partidoId);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para registrar goles en este partido.");

        if (partido.Estado == "Pendiente" || partido.Estado == "Suspendido")
            throw new Exception("No se pueden registrar goles en un partido pendiente o suspendido.");

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

        var gol = new Gol
        {
            PartidoId = partidoId,
            JugadorId = request.JugadorId,
            EquipoId = request.EquipoId,
            Minuto = request.Minuto,
            EsAutogol = request.EsAutogol
        };

        _context.Goles.Add(gol);
        await _context.SaveChangesAsync();

        // Recargar con includes para la respuesta
        await _context.Entry(gol).Reference(g => g.Jugador).LoadAsync();
        await _context.Entry(gol).Reference(g => g.Equipo).LoadAsync();

        return MapToResponse(gol);
    }

    // Anular gol
    public async Task AnularAsync(int golId, int organizadorId)
    {
        var gol = await _context.Goles
            .Include(g => g.Partido)
                .ThenInclude(p => p.Torneo)
            .FirstOrDefaultAsync(g => g.Id == golId);

        if (gol is null)
            throw new Exception("Gol no encontrado.");

        if (gol.Partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para anular este gol.");

        _context.Goles.Remove(gol);
        await _context.SaveChangesAsync();
    }
}