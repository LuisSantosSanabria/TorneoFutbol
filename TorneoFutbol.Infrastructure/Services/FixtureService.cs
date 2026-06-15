using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Fixture;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class FixtureService : IFixtureService
{
    private readonly TorneoFutbolDbContext _context;

    public FixtureService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Helper para mapear Partido → PartidoResponse
    private static PartidoResponse MapToResponse(Partido p) => new()
    {
        Id = p.Id,
        FechaNumero = p.FechaNumero,
        FechaHora = p.FechaHora,
        Cancha = p.Cancha,
        Estado = p.Estado,
        TorneoId = p.TorneoId,
        NombreTorneo = p.Torneo.Nombre,
        EquipoLocalId = p.EquipoLocalId,
        NombreEquipoLocal = p.EquipoLocal.Nombre,
        EquipoVisitanteId = p.EquipoVisitanteId,
        NombreEquipoVisitante = p.EquipoVisitante.Nombre,
        GolesLocal = p.GolesLocal,
        GolesVisitante = p.GolesVisitante,
        Resultado = p.GolesLocal.HasValue && p.GolesVisitante.HasValue
            ? $"{p.GolesLocal} - {p.GolesVisitante}"
            : "Pendiente"
    };

    // Helper para cargar partido con includes
    private async Task<Partido?> CargarPartidoAsync(int id) =>
        await _context.Partidos
            .Include(p => p.Torneo)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .FirstOrDefaultAsync(p => p.Id == id);

    // Obtener fixture completo agrupado por fecha
    public async Task<IEnumerable<FechaResponse>> GetFixtureByTorneoAsync(int torneoId)
    {
        var partidos = await _context.Partidos
            .Include(p => p.Torneo)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Where(p => p.TorneoId == torneoId)
            .OrderBy(p => p.FechaNumero)
            .ThenBy(p => p.FechaHora)
            .ToListAsync();

        return partidos
            .GroupBy(p => p.FechaNumero)
            .Select(g => new FechaResponse
            {
                FechaNumero = g.Key,
                Partidos = g.Select(MapToResponse).ToList()
            })
            .ToList();
    }

    // Obtener partidos de una fecha
    public async Task<IEnumerable<PartidoResponse>> GetByFechaAsync(int torneoId, int fechaNumero)
    {
        var partidos = await _context.Partidos
            .Include(p => p.Torneo)
            .Include(p => p.EquipoLocal)
            .Include(p => p.EquipoVisitante)
            .Where(p => p.TorneoId == torneoId && p.FechaNumero == fechaNumero)
            .OrderBy(p => p.FechaHora)
            .ToListAsync();

        return partidos.Select(MapToResponse);
    }

    // Obtener partido por Id
    public async Task<PartidoResponse?> GetPartidoByIdAsync(int id)
    {
        var partido = await CargarPartidoAsync(id);
        return partido is null ? null : MapToResponse(partido);
    }

    // Generar fixture automático Round-Robin
    public async Task<IEnumerable<FechaResponse>> GenerarFixtureAutomaticoAsync(
        GenerarFixtureRequest request, int organizadorId)
    {
        var torneo = await _context.Torneos
            .Include(t => t.Equipos)
            .FirstOrDefaultAsync(t => t.Id == request.TorneoId);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para generar el fixture de este torneo.");

        if (torneo.Equipos.Count < 2)
            throw new Exception("El torneo necesita al menos 2 equipos para generar el fixture.");

        // Verificar que no haya fixture generado previamente
        var tienePartidos = await _context.Partidos
            .AnyAsync(p => p.TorneoId == request.TorneoId);

        if (tienePartidos)
            throw new Exception("Ya existe un fixture para este torneo. Eliminá los partidos existentes primero.");

        var equipos = torneo.Equipos.ToList();

        // Si N es impar agregamos un equipo fantasma (bye)
        bool tieneEquipoFantasma = equipos.Count % 2 != 0;
        if (tieneEquipoFantasma)
            equipos.Add(new Equipo { Id = -1, Nombre = "BYE" });

        int n = equipos.Count;
        var partidos = new List<Partido>();
        var fechaActual = request.FechaInicioPrimerFecha;

        for (int fecha = 1; fecha < n; fecha++)
        {
            for (int i = 0; i < n / 2; i++)
            {
                var local = equipos[i];
                var visitante = equipos[n - 1 - i];

                // Saltamos los partidos con el equipo fantasma
                if (local.Id == -1 || visitante.Id == -1) continue;

                partidos.Add(new Partido
                {
                    FechaNumero = fecha,
                    FechaHora = fechaActual,
                    Cancha = request.Cancha,
                    Estado = "Pendiente",
                    TorneoId = request.TorneoId,
                    EquipoLocalId = local.Id,
                    EquipoVisitanteId = visitante.Id
                });
            }

            // Rotar equipos fijando el primero
            var ultimo = equipos[n - 1];
            equipos.RemoveAt(n - 1);
            equipos.Insert(1, ultimo);

            fechaActual = fechaActual.AddDays(request.DiasEntrePartidos);
        }

        _context.Partidos.AddRange(partidos);
        await _context.SaveChangesAsync();

        return await GetFixtureByTorneoAsync(request.TorneoId);
    }

    // Crear partido manual
    public async Task<PartidoResponse> CrearPartidoAsync(
        CrearPartidoRequest request, int organizadorId)
    {
        var torneo = await _context.Torneos
            .FirstOrDefaultAsync(t => t.Id == request.TorneoId);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para agregar partidos a este torneo.");

        if (torneo.Estado == "Finalizado")
            throw new Exception("No se pueden agregar partidos a un torneo finalizado.");

        if (request.EquipoLocalId == request.EquipoVisitanteId)
            throw new Exception("El equipo local y visitante no pueden ser el mismo.");

        // Verificar que ambos equipos pertenecen al torneo
        var equiposDelTorneo = await _context.Equipos
            .Where(e => e.TorneoId == request.TorneoId)
            .Select(e => e.Id)
            .ToListAsync();

        if (!equiposDelTorneo.Contains(request.EquipoLocalId))
            throw new Exception("El equipo local no pertenece a este torneo.");

        if (!equiposDelTorneo.Contains(request.EquipoVisitanteId))
            throw new Exception("El equipo visitante no pertenece a este torneo.");

        // Verificar que no exista el mismo partido en la misma fecha
        var partidoDuplicado = await _context.Partidos
            .AnyAsync(p => p.TorneoId == request.TorneoId &&
                          p.FechaNumero == request.FechaNumero &&
                          ((p.EquipoLocalId == request.EquipoLocalId &&
                            p.EquipoVisitanteId == request.EquipoVisitanteId) ||
                           (p.EquipoLocalId == request.EquipoVisitanteId &&
                            p.EquipoVisitanteId == request.EquipoLocalId)));

        if (partidoDuplicado)
            throw new Exception("Ya existe un partido entre estos equipos en esta fecha.");

        var partido = new Partido
        {
            FechaNumero = request.FechaNumero,
            FechaHora = request.FechaHora,
            Cancha = request.Cancha,
            Estado = "Pendiente",
            TorneoId = request.TorneoId,
            EquipoLocalId = request.EquipoLocalId,
            EquipoVisitanteId = request.EquipoVisitanteId
        };

        _context.Partidos.Add(partido);
        await _context.SaveChangesAsync();

        var partidoCreado = await CargarPartidoAsync(partido.Id);
        return MapToResponse(partidoCreado!);
    }

    // Editar partido
    public async Task<PartidoResponse> EditarPartidoAsync(
        int id, EditarPartidoRequest request, int organizadorId)
    {
        var partido = await CargarPartidoAsync(id);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para editar este partido.");

        if (partido.Estado == "Finalizado")
            throw new Exception("No se puede editar un partido finalizado.");

        if (request.EquipoLocalId == request.EquipoVisitanteId)
            throw new Exception("El equipo local y visitante no pueden ser el mismo.");

        partido.FechaNumero = request.FechaNumero;
        partido.FechaHora = request.FechaHora;
        partido.Cancha = request.Cancha;
        partido.EquipoLocalId = request.EquipoLocalId;
        partido.EquipoVisitanteId = request.EquipoVisitanteId;

        await _context.SaveChangesAsync();

        var partidoActualizado = await CargarPartidoAsync(id);
        return MapToResponse(partidoActualizado!);
    }

    // Eliminar partido
    public async Task EliminarPartidoAsync(int id, int organizadorId)
    {
        var partido = await CargarPartidoAsync(id);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para eliminar este partido.");

        if (partido.Estado != "Pendiente")
            throw new Exception("Solo se pueden eliminar partidos pendientes.");

        _context.Partidos.Remove(partido);
        await _context.SaveChangesAsync();
    }

    // Cargar resultado
    public async Task<PartidoResponse> CargarResultadoAsync(
        int id, CargarResultadoRequest request, int organizadorId)
    {
        var partido = await CargarPartidoAsync(id);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para cargar resultados en este torneo.");

        if (partido.Estado == "Pendiente")
            throw new Exception("El partido debe estar En Juego para cargar el resultado.");

        if (partido.Estado == "Finalizado")
            throw new Exception("El partido ya tiene resultado cargado.");

        if (request.GolesLocal < 0 || request.GolesVisitante < 0)
            throw new Exception("Los goles no pueden ser negativos.");

        partido.GolesLocal = request.GolesLocal;
        partido.GolesVisitante = request.GolesVisitante;
        partido.Estado = "Finalizado";

        await _context.SaveChangesAsync();

        return MapToResponse(partido);
    }

    // Cambiar estado del partido
    public async Task<PartidoResponse> CambiarEstadoPartidoAsync(
        int id, string nuevoEstado, int organizadorId)
    {
        var partido = await CargarPartidoAsync(id);

        if (partido is null)
            throw new Exception("Partido no encontrado.");

        if (partido.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para modificar este partido.");

        var transicionesValidas = new Dictionary<string, List<string>>
        {
            { "Pendiente",   new List<string> { "EnJuego", "Suspendido" } },
            { "EnJuego",     new List<string> { "Finalizado", "Suspendido" } },
            { "Suspendido",  new List<string> { "Pendiente" } }
        };

        if (!transicionesValidas.TryGetValue(partido.Estado, out var estadosPermitidos)
            || !estadosPermitidos.Contains(nuevoEstado))
            throw new Exception($"No se puede pasar de '{partido.Estado}' a '{nuevoEstado}'.");

        partido.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return MapToResponse(partido);
    }
}