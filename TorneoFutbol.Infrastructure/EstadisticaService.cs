using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Estadisticas;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class EstadisticaService : IEstadisticaService
{
    private readonly TorneoFutbolDbContext _context;

    public EstadisticaService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Tabla de Posiciones
    public async Task<IEnumerable<PosicionResponse>> GetTablaPosicionesAsync(int torneoId)
    {
        // Traer todos los equipos del torneo
        var equipos = await _context.Equipos
            .Where(e => e.TorneoId == torneoId)
            .ToListAsync();

        // Traer todos los partidos finalizados del torneo
        var partidos = await _context.Partidos
            .Where(p => p.TorneoId == torneoId && p.Estado == "Finalizado")
            .ToListAsync();

        // Calcular estadísticas por equipo
        var tabla = equipos.Select(equipo =>
        {
            // Partidos como local
            var comoLocal = partidos
                .Where(p => p.EquipoLocalId == equipo.Id)
                .ToList();

            // Partidos como visitante
            var comoVisitante = partidos
                .Where(p => p.EquipoVisitanteId == equipo.Id)
                .ToList();

            int pj = comoLocal.Count + comoVisitante.Count;

            int pg = comoLocal.Count(p => p.GolesLocal > p.GolesVisitante) +
                     comoVisitante.Count(p => p.GolesVisitante > p.GolesLocal);

            int pe = comoLocal.Count(p => p.GolesLocal == p.GolesVisitante) +
                     comoVisitante.Count(p => p.GolesLocal == p.GolesVisitante);

            int pp = comoLocal.Count(p => p.GolesLocal < p.GolesVisitante) +
                     comoVisitante.Count(p => p.GolesVisitante < p.GolesLocal);

            int gf = comoLocal.Sum(p => p.GolesLocal ?? 0) +
                     comoVisitante.Sum(p => p.GolesVisitante ?? 0);

            int gc = comoLocal.Sum(p => p.GolesVisitante ?? 0) +
                     comoVisitante.Sum(p => p.GolesLocal ?? 0);

            return new PosicionResponse
            {
                EquipoId = equipo.Id,
                NombreEquipo = equipo.Nombre,
                EscudoUrl = equipo.EscudoUrl,
                PJ = pj,
                PG = pg,
                PE = pe,
                PP = pp,
                GF = gf,
                GC = gc,
                DG = gf - gc,
                PTS = (pg * 3) + (pe * 1)
            };
        })
        // Ordenar por puntos, después diferencia de goles, después goles a favor
        .OrderByDescending(p => p.PTS)
        .ThenByDescending(p => p.DG)
        .ThenByDescending(p => p.GF)
        .ToList();

        // Asignar posición en la tabla
        for (int i = 0; i < tabla.Count; i++)
            tabla[i].Posicion = i + 1;

        return tabla;
    }

    // Tabla de Goleadores
    public async Task<IEnumerable<GoleadorResponse>> GetTablaGoleadoresAsync(int torneoId)
    {
        // Traer goles de partidos del torneo
        var goles = await _context.Goles
            .Include(g => g.Jugador)
                .ThenInclude(j => j.Equipo)
            .Include(g => g.Partido)
            .Where(g => g.Partido.TorneoId == torneoId)
            .ToListAsync();

        // Agrupar por jugador y contar goles
        var tabla = goles
            .GroupBy(g => g.JugadorId)
            .Select(g =>
            {
                var jugador = g.First().Jugador;
                return new GoleadorResponse
                {
                    JugadorId = jugador.Id,
                    NombreJugador = jugador.Nombre,
                    NombreEquipo = jugador.Equipo.Nombre,
                    Posicion_Juego = jugador.Posicion,
                    Goles = g.Count(x => !x.EsAutogol),
                    GolesAutogol = g.Count(x => x.EsAutogol)
                };
            })
            .OrderByDescending(g => g.Goles)
            .ToList();

        // Asignar posición en el ranking
        for (int i = 0; i < tabla.Count; i++)
            tabla[i].Posicion = i + 1;

        return tabla;
    }
}