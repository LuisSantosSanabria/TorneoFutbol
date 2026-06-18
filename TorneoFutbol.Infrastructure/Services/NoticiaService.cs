using Microsoft.EntityFrameworkCore;
using TorneoFutbol.Core.DTOs.Noticias;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;

namespace TorneoFutbol.Infrastructure.Services;

public class NoticiaService : INoticiaService
{
    private readonly TorneoFutbolDbContext _context;

    public NoticiaService(TorneoFutbolDbContext context)
    {
        _context = context;
    }

    private static NoticiaResponse MapToResponse(Noticia n) => new()
    {
        Id = n.Id,
        Titulo = n.Titulo,
        Contenido = n.Contenido,
        FechaPublicacion = n.FechaPublicacion,
        TorneoId = n.TorneoId,
        NombreTorneo = n.Torneo.Nombre
    };

    // Obtener noticias de un torneo
    public async Task<IEnumerable<NoticiaResponse>> GetByTorneoAsync(int torneoId)
    {
        var noticias = await _context.Noticias
            .Include(n => n.Torneo)
            .Where(n => n.TorneoId == torneoId)
            .OrderByDescending(n => n.FechaPublicacion)
            .ToListAsync();

        return noticias.Select(MapToResponse);
    }

    // Obtener por Id
    public async Task<NoticiaResponse?> GetByIdAsync(int id)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Torneo)
            .FirstOrDefaultAsync(n => n.Id == id);

        return noticia is null ? null : MapToResponse(noticia);
    }

    // Crear
    public async Task<NoticiaResponse> CrearAsync(CrearNoticiaRequest request, int organizadorId)
    {
        var torneo = await _context.Torneos
            .FirstOrDefaultAsync(t => t.Id == request.TorneoId);

        if (torneo is null)
            throw new Exception("Torneo no encontrado.");

        if (torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para publicar noticias en este torneo.");

        var noticia = new Noticia
        {
            Titulo = request.Titulo,
            Contenido = request.Contenido,
            FechaPublicacion = DateTime.UtcNow,
            TorneoId = request.TorneoId
        };

        _context.Noticias.Add(noticia);
        await _context.SaveChangesAsync();

        await _context.Entry(noticia).Reference(n => n.Torneo).LoadAsync();

        return MapToResponse(noticia);
    }

    // Editar
    public async Task<NoticiaResponse> EditarAsync(int id, EditarNoticiaRequest request, int organizadorId)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Torneo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (noticia is null)
            throw new Exception("Noticia no encontrada.");

        if (noticia.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para editar esta noticia.");

        noticia.Titulo = request.Titulo;
        noticia.Contenido = request.Contenido;

        await _context.SaveChangesAsync();

        return MapToResponse(noticia);
    }

    // Eliminar
    public async Task EliminarAsync(int id, int organizadorId)
    {
        var noticia = await _context.Noticias
            .Include(n => n.Torneo)
            .FirstOrDefaultAsync(n => n.Id == id);

        if (noticia is null)
            throw new Exception("Noticia no encontrada.");

        if (noticia.Torneo.OrganizadorId != organizadorId)
            throw new Exception("No tenés permisos para eliminar esta noticia.");

        _context.Noticias.Remove(noticia);
        await _context.SaveChangesAsync();
    }
}