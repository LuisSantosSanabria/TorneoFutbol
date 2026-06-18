using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Noticias;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoticiaController : ControllerBase
{
    private readonly INoticiaService _noticiaService;

    public NoticiaController(INoticiaService noticiaService)
    {
        _noticiaService = noticiaService;
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/noticia/torneo/1
    [HttpGet("torneo/{torneoId}")]
    public async Task<IActionResult> GetByTorneo(int torneoId)
    {
        var noticias = await _noticiaService.GetByTorneoAsync(torneoId);
        return Ok(noticias);
    }

    // GET api/noticia/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var noticia = await _noticiaService.GetByIdAsync(id);

        if (noticia is null)
            return NotFound(new { mensaje = "Noticia no encontrada." });

        return Ok(noticia);
    }

    // POST api/noticia
    [HttpPost]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Crear(CrearNoticiaRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var noticia = await _noticiaService.CrearAsync(request, organizadorId);
            return CreatedAtAction(nameof(GetById), new { id = noticia.Id }, noticia);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT api/noticia/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Editar(int id, EditarNoticiaRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var noticia = await _noticiaService.EditarAsync(id, request, organizadorId);
            return Ok(noticia);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/noticia/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _noticiaService.EliminarAsync(id, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}