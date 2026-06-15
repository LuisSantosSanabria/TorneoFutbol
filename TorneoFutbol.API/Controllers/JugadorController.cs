using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Jugadores;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JugadorController : ControllerBase
{
    private readonly IJugadorService _jugadorService;

    public JugadorController(IJugadorService jugadorService)
    {
        _jugadorService = jugadorService;
    }

    // Helpers
    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/jugador/equipo/5
    [HttpGet("equipo/{equipoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByEquipo(int equipoId)
    {
        var jugadores = await _jugadorService.GetByEquipoAsync(equipoId);
        return Ok(jugadores);
    }

    // GET api/jugador/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var jugador = await _jugadorService.GetByIdAsync(id);

        if (jugador is null)
            return NotFound(new { mensaje = "Jugador no encontrado." });

        return Ok(jugador);
    }

    // POST api/jugador
    [HttpPost]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Alta(AltaJugadorRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var jugador = await _jugadorService.AltaAsync(request, organizadorId);
            return CreatedAtAction(nameof(GetById), new { id = jugador.Id }, jugador);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT api/jugador/5
    [HttpPut("{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Editar(int id, EditarJugadorRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var jugador = await _jugadorService.EditarAsync(id, request, organizadorId);
            return Ok(jugador);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PATCH api/jugador/5/baja
    [HttpPatch("{id}/baja")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Baja(int id)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _jugadorService.BajaAsync(id, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PATCH api/jugador/5/transferir
    [HttpPatch("{id}/transferir")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Transferir(int id, TransferirJugadorRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var jugador = await _jugadorService.TransferirAsync(id, request, organizadorId);
            return Ok(jugador);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}