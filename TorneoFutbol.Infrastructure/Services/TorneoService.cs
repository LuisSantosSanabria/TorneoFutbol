using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Torneos;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class TorneoService : ITorneoService
{
    private readonly TorneoFutbolDbContext _context;

    public TorneoService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    // Obtener todos 
    public async Task<IEnumerable<TorneoResponse>> GetAllAsync()
    {
        return await _context.Torneos
            .Include(t => t.Organizador)
            .Include(t => t.Equipos)
            .Select(t => new TorneoResponse
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin,
                Estado = t.Estado,
                NombreOrganizador = t.Organizador.Nombre,
                CantidadEquipos = t.Equipos.Count
            })
            .ToListAsync();
    }

    // Obtener por Id
    public async Task<TorneoResponse?> GetByIdAsync(int id)
    {
        var torneo = await _context.Torneos
            .Include(t => t.Organizador)
            .Include(t => t.Equipos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (torneo is null) return null;

        return new TorneoResponse
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Descripcion = torneo.Descripcion,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            Estado = torneo.Estado,
            NombreOrganizador = torneo.Organizador.Nombre,
            CantidadEquipos = torneo.Equipos.Count
        };
    }

    // Crear
    public async Task<TorneoResponse> CrearAsync(CrearTorneoRequest request, int organizadorId)
    {
        var torneo = new Torneo
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Estado = "Planificado",
            OrganizadorId = organizadorId
        };

        _context.Torneos.Add(torneo);
        await _context.SaveChangesAsync();

        // Recargar con el organizador incluido para la respuesta
        await _context.Entry(torneo)
            .Reference(t => t.Organizador)
            .LoadAsync();

        return new TorneoResponse
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Descripcion = torneo.Descripcion,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            Estado = torneo.Estado,
            NombreOrganizador = torneo.Organizador.Nombre,
            CantidadEquipos = 0
        };
    }

    // Editar
    public async Task<TorneoResponse> EditarAsync(int id, EditarTorneoRequest request, int organizadorId)
    {
        var torneo = await _context.Torneos
            .Include(t => t.Organizador)
            .Include(t => t.Equipos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para editar este torneo.");

        if (torneo.Estado == "Finalizado")
            throw new Exception("No se puede editar un torneo finalizado.");

        torneo.Nombre = request.Nombre;
        torneo.Descripcion = request.Descripcion;
        torneo.FechaInicio = request.FechaInicio;
        torneo.FechaFin = request.FechaFin;

        await _context.SaveChangesAsync();

        return new TorneoResponse
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Descripcion = torneo.Descripcion,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            Estado = torneo.Estado,
            NombreOrganizador = torneo.Organizador.Nombre,
            CantidadEquipos = torneo.Equipos.Count
        };
    }

    // Eliminar
    public async Task EliminarAsync(int id, int organizadorId)
    {
        var torneo = await _context.Torneos
            .FirstOrDefaultAsync(t => t.Id == id);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para eliminar este torneo.");

        if (torneo.Estado == "EnCurso")
            throw new Exception("No se puede eliminar un torneo en curso.");

        _context.Torneos.Remove(torneo);
        await _context.SaveChangesAsync();
    }

    // Cambiar Estado
    public async Task<TorneoResponse> CambiarEstadoAsync(int id, string nuevoEstado, int organizadorId)
    {
        var torneo = await _context.Torneos
            .Include(t => t.Organizador)
            .Include(t => t.Equipos)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para modificar este torneo.");

        // Validar transiciones de estado permitidas
        var transicionesValidas = new Dictionary<string, string>
        {
            { "Planificado", "EnCurso" },
            { "EnCurso", "Finalizado" }
        };

        if (!transicionesValidas.TryGetValue(torneo.Estado, out var estadoPermitido)
            || estadoPermitido != nuevoEstado)
            throw new Exception($"No se puede pasar de '{torneo.Estado}' a '{nuevoEstado}'.");

        torneo.Estado = nuevoEstado;
        await _context.SaveChangesAsync();

        return new TorneoResponse
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Descripcion = torneo.Descripcion,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            Estado = torneo.Estado,
            NombreOrganizador = torneo.Organizador.Nombre,
            CantidadEquipos = torneo.Equipos.Count
        };
    }
}