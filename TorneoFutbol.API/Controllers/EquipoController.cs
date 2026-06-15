using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Equipos;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EquipoController : ControllerBase
{
    private readonly IEquipoService _equipoService;

    public EquipoController(IEquipoService equipoService)
    {
        _equipoService = equipoService;
    }

    // los Helpers son métodos privados que ayudan a evitar repetir código, en este caso para obtener el Id del usuario logueado
    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/equipo/torneo/5
    [HttpGet("torneo/{torneoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTorneo(int torneoId)
    {
        var equipos = await _equipoService.GetByTorneoAsync(torneoId);
        return Ok(equipos);
    }

    // GET api/equipo/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var equipo = await _equipoService.GetByIdAsync(id);

        if (equipo is null)
            return NotFound(new { mensaje = "Equipo no encontrado." });

        return Ok(equipo);
    }

    // POST api/equipo
    [HttpPost]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Crear(CrearEquipoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var equipo = await _equipoService.CrearAsync(request, organizadorId);
            return CreatedAtAction(nameof(GetById), new { id = equipo.Id }, equipo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT api/equipo/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Editar(int id, EditarEquipoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var equipo = await _equipoService.EditarAsync(id, request, organizadorId);
            return Ok(equipo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/equipo/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _equipoService.EliminarAsync(id, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}