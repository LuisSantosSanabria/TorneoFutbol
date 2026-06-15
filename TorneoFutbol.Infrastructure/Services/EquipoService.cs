using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Equipos;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly TorneoFutbolDbContext _context;

        public EquipoService(TorneoFutbolDbContext context)
        {
            _context = context;
        }

        // Obtener equipos de un torneo
        public async Task<IEnumerable<EquipoResponse>> GetByTorneoAsync(int torneoId)
        {
            return await _context.Equipos
                .Include(e => e.Torneo)
                .Include(e => e.Jugadores)
                .Where(e => e.TorneoId == torneoId)
                .Select(e => new EquipoResponse
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    EscudoUrl = e.EscudoUrl,
                    TorneoId = e.TorneoId,
                    NombreTorneo = e.Torneo.Nombre,
                    CantidadJugadores = e.Jugadores.Count
                })
                .ToListAsync();
        }

        // Obtener por Id
        public async Task<EquipoResponse?> GetByIdAsync(int id)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Torneo)
                .Include(e => e.Jugadores)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo is null) return null;

            return new EquipoResponse
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                EscudoUrl = equipo.EscudoUrl,
                TorneoId = equipo.TorneoId,
                NombreTorneo = equipo.Torneo.Nombre,
                CantidadJugadores = equipo.Jugadores.Count
            };
        }

        // Crear
        public async Task<EquipoResponse> CrearAsync(CrearEquipoRequest request, int organizadorId)
        {
            // Verificar que el torneo existe y pertenece al organizador
            var torneo = await _context.Torneos
                .FirstOrDefaultAsync(t => t.Id == request.TorneoId);

            if (torneo is null)
                throw new Exception("Torneo no encontrado.");

            if (torneo.OrganizadorId != organizadorId)
                throw new Exception("No tenés permisos para agregar equipos a este torneo.");

            if (torneo.Estado == "Finalizado")
                throw new Exception("No se pueden agregar equipos a un torneo finalizado.");

            // Verificar que no exista otro equipo con el mismo nombre en el torneo
            var nombreExiste = await _context.Equipos
                .AnyAsync(e => e.TorneoId == request.TorneoId &&
                              e.Nombre.ToLower() == request.Nombre.ToLower());

            if (nombreExiste)
                throw new Exception("Ya existe un equipo con ese nombre en el torneo.");

            var equipo = new Equipo
            {
                Nombre = request.Nombre,
                EscudoUrl = request.EscudoUrl,
                TorneoId = request.TorneoId
            };

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            await _context.Entry(equipo)
                .Reference(e => e.Torneo)
                .LoadAsync();

            return new EquipoResponse
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                EscudoUrl = equipo.EscudoUrl,
                TorneoId = equipo.TorneoId,
                NombreTorneo = equipo.Torneo.Nombre,
                CantidadJugadores = 0
            };
        }

        // Editar
        public async Task<EquipoResponse> EditarAsync(int id, EditarEquipoRequest request, int organizadorId)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Torneo)
                .Include(e => e.Jugadores)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo is null)
                throw new Exception("Equipo no encontrado.");

            if (equipo.Torneo.OrganizadorId != organizadorId)
                throw new Exception("No tenés permisos para editar este equipo.");

            if (equipo.Torneo.Estado == "Finalizado")
                throw new Exception("No se puede editar un equipo de un torneo finalizado.");

            // Verificar nombre duplicado (excluyendo el equipo actual)
            var nombreExiste = await _context.Equipos
                .AnyAsync(e => e.TorneoId == equipo.TorneoId &&
                              e.Nombre.ToLower() == request.Nombre.ToLower() &&
                              e.Id != id);

            if (nombreExiste)
                throw new Exception("Ya existe un equipo con ese nombre en el torneo.");

            equipo.Nombre = request.Nombre;
            equipo.EscudoUrl = request.EscudoUrl;

            await _context.SaveChangesAsync();

            return new EquipoResponse
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                EscudoUrl = equipo.EscudoUrl,
                TorneoId = equipo.TorneoId,
                NombreTorneo = equipo.Torneo.Nombre,
                CantidadJugadores = equipo.Jugadores.Count
            };
        }

        // Eliminar
        public async Task EliminarAsync(int id, int organizadorId)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Torneo)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (equipo is null)
                throw new Exception("Equipo no encontrado.");

            if (equipo.Torneo.OrganizadorId != organizadorId)
                throw new Exception("No tenés permisos para eliminar este equipo.");

            if (equipo.Torneo.Estado == "EnCurso")
                throw new Exception("No se puede eliminar un equipo con el torneo en curso.");

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();
        }
    }
}
