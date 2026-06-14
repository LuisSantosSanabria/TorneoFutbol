using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Torneos;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // requieren JWT
public class TorneoController : ControllerBase
{
    private readonly ITorneoService _torneoService;

    public TorneoController(ITorneoService torneoService)
    {
        _torneoService = torneoService;
    }

    // los Helpers son métodos privados que nos ayudan a obtener información del usuario autenticado, como su ID o rol, a partir de los claims del JWT.
    // Esto es útil para verificar permisos o asociar acciones con el usuario correcto.
    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    private string GetUsuarioRol()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    // GET api/torneo
    [HttpGet]
    [AllowAnonymous] // Cualquiera puede ver los torneos
    public async Task<IActionResult> GetAll()
    {
        var torneos = await _torneoService.GetAllAsync();
        return Ok(torneos);
    }

    // GET api/torneo/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var torneo = await _torneoService.GetByIdAsync(id);

        if (torneo is null)
            return NotFound(new { mensaje = "Torneo no encontrado." });

        return Ok(torneo);
    }

    // POST api/torneo
    [HttpPost]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Crear(CrearTorneoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var torneo = await _torneoService.CrearAsync(request, organizadorId);
            return CreatedAtAction(nameof(GetById), new { id = torneo.Id }, torneo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT api/torneo/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Editar(int id, EditarTorneoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var torneo = await _torneoService.EditarAsync(id, request, organizadorId);
            return Ok(torneo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/torneo/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _torneoService.EliminarAsync(id, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PATCH api/torneo/5/estado
    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var torneo = await _torneoService.CambiarEstadoAsync(id, nuevoEstado, organizadorId);
            return Ok(torneo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}