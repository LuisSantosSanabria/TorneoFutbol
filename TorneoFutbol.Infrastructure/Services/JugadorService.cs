using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Jugadores;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class JugadorService : IJugadorService
{
    private readonly TorneoFutbolDbContext _context;

    public JugadorService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Obtener jugadores de un equipo
    public async Task<IEnumerable<JugadorResponse>> GetByEquipoAsync(int equipoId)
    {
        return await _context.Jugadores
            .Include(j => j.Equipo)
            .Where(j => j.EquipoId == equipoId)
            .Select(j => new JugadorResponse
            {
                Id = j.Id,
                Nombre = j.Nombre,
                NumeroCamiseta = j.NumeroCamiseta,
                Posicion = j.Posicion,
                FotoUrl = j.FotoUrl,
                Activo = j.Activo,
                EquipoId = j.EquipoId,
                NombreEquipo = j.Equipo.Nombre
            })
            .ToListAsync();
    }

    // Obtener por Id
    public async Task<JugadorResponse?> GetByIdAsync(int id)
    {
        var jugador = await _context.Jugadores
            .Include(j => j.Equipo)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jugador is null) return null;

        return new JugadorResponse
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            NumeroCamiseta = jugador.NumeroCamiseta,
            Posicion = jugador.Posicion,
            FotoUrl = jugador.FotoUrl,
            Activo = jugador.Activo,
            EquipoId = jugador.EquipoId,
            NombreEquipo = jugador.Equipo.Nombre
        };
    }

    // Alta
    public async Task<JugadorResponse> AltaAsync(AltaJugadorRequest request, int organizadorId)
    {
        var equipo = await _context.Equipos
            .Include(e => e.Torneo)
            .FirstOrDefaultAsync(e => e.Id == request.EquipoId);

        if (equipo is null)
            throw new Exception("Equipo no encontrado.");

        if (equipo.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para agregar jugadores a este equipo.");

        if (equipo.Torneo.Estado == "Finalizado")
            throw new Exception("No se pueden agregar jugadores a un torneo finalizado.");

        // Verificar número de camiseta duplicado en el equipo
        var camisetaExiste = await _context.Jugadores
            .AnyAsync(j => j.EquipoId == request.EquipoId &&
                          j.NumeroCamiseta == request.NumeroCamiseta &&
                          j.Activo);

        if (camisetaExiste)
            throw new Exception($"Ya existe un jugador activo con la camiseta #{request.NumeroCamiseta} en este equipo.");

        var jugador = new Jugador
        {
            Nombre = request.Nombre,
            NumeroCamiseta = request.NumeroCamiseta,
            Posicion = request.Posicion,
            EquipoId = request.EquipoId,
            Activo = true
        };

        _context.Jugadores.Add(jugador);
        await _context.SaveChangesAsync();

        await _context.Entry(jugador)
            .Reference(j => j.Equipo)
            .LoadAsync();

        return new JugadorResponse
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            NumeroCamiseta = jugador.NumeroCamiseta,
            Posicion = jugador.Posicion,
            FotoUrl = jugador.FotoUrl,
            Activo = jugador.Activo,
            EquipoId = jugador.EquipoId,
            NombreEquipo = jugador.Equipo.Nombre
        };
    }

    // Editar
    public async Task<JugadorResponse> EditarAsync(int id, EditarJugadorRequest request, int organizadorId)
    {
        var jugador = await _context.Jugadores
            .Include(j => j.Equipo)
                .ThenInclude(e => e.Torneo)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jugador is null)
            throw new Exception("Jugador no encontrado.");

        if (jugador.Equipo.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para editar este jugador.");

        // Verificar camiseta duplicada excluyendo el jugador actual
        var camisetaExiste = await _context.Jugadores
            .AnyAsync(j => j.EquipoId == jugador.EquipoId &&
                          j.NumeroCamiseta == request.NumeroCamiseta &&
                          j.Activo &&
                          j.Id != id);

        if (camisetaExiste)
            throw new Exception($"Ya existe un jugador activo con la camiseta #{request.NumeroCamiseta} en este equipo.");

        jugador.Nombre = request.Nombre;
        jugador.NumeroCamiseta = request.NumeroCamiseta;
        jugador.Posicion = request.Posicion;

        await _context.SaveChangesAsync();

        return new JugadorResponse
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            NumeroCamiseta = jugador.NumeroCamiseta,
            Posicion = jugador.Posicion,
            FotoUrl = jugador.FotoUrl,
            Activo = jugador.Activo,
            EquipoId = jugador.EquipoId,
            NombreEquipo = jugador.Equipo.Nombre
        };
    }

    // esta operación no borra el jugador de la base, solo lo marca como inactivo para mantener su historial en goles y partidos
    public async Task BajaAsync(int id, int organizadorId)
    {
        var jugador = await _context.Jugadores
            .Include(j => j.Equipo)
                .ThenInclude(e => e.Torneo)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jugador is null)
            throw new Exception("Jugador no encontrado.");

        if (jugador.Equipo.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para dar de baja este jugador.");

        if (!jugador.Activo)
            throw new Exception("El jugador ya está dado de baja.");

        jugador.Activo = false;
        await _context.SaveChangesAsync();
    }

    // Transferir
    public async Task<JugadorResponse> TransferirAsync(int id, TransferirJugadorRequest request, int organizadorId)
    {
        var jugador = await _context.Jugadores
            .Include(j => j.Equipo)
                .ThenInclude(e => e.Torneo)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (jugador is null)
            throw new Exception("Jugador no encontrado.");

        if (jugador.Equipo.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para transferir este jugador.");

        if (!jugador.Activo)
            throw new Exception("No se puede transferir un jugador dado de baja.");

        // Verificar que el equipo destino existe y es del mismo torneo
        var equipoDestino = await _context.Equipos
            .Include(e => e.Torneo)
            .FirstOrDefaultAsync(e => e.Id == request.EquipoDestinoId);

        if (equipoDestino is null)
            throw new Exception("Equipo destino no encontrado.");

        if (equipoDestino.TorneoId != jugador.Equipo.TorneoId)
            throw new Exception("Solo se puede transferir jugadores dentro del mismo torneo.");

        // Verificar que no haya camiseta duplicada en el equipo destino
        var camisetaExiste = await _context.Jugadores
            .AnyAsync(j => j.EquipoId == request.EquipoDestinoId &&
                          j.NumeroCamiseta == jugador.NumeroCamiseta &&
                          j.Activo);

        if (camisetaExiste)
            throw new Exception($"Ya existe un jugador con la camiseta #{jugador.NumeroCamiseta} en el equipo destino.");

        jugador.EquipoId = request.EquipoDestinoId;
        await _context.SaveChangesAsync();

        await _context.Entry(jugador)
            .Reference(j => j.Equipo)
            .LoadAsync();

        return new JugadorResponse
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            NumeroCamiseta = jugador.NumeroCamiseta,
            Posicion = jugador.Posicion,
            FotoUrl = jugador.FotoUrl,
            Activo = jugador.Activo,
            EquipoId = jugador.EquipoId,
            NombreEquipo = jugador.Equipo.Nombre
        };
    }
}