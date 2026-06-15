using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Fixture;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FixtureController : ControllerBase
{
    private readonly IFixtureService _fixtureService;

    public FixtureController(IFixtureService fixtureService)
    {
        _fixtureService = fixtureService;
    }

    // Helpers
    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/fixture/torneo/1
    [HttpGet("torneo/{torneoId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFixture(int torneoId)
    {
        var fixture = await _fixtureService.GetFixtureByTorneoAsync(torneoId);
        return Ok(fixture);
    }

    // GET api/fixture/torneo/1/fecha/2
    [HttpGet("torneo/{torneoId}/fecha/{fechaNumero}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByFecha(int torneoId, int fechaNumero)
    {
        var partidos = await _fixtureService.GetByFechaAsync(torneoId, fechaNumero);
        return Ok(partidos);
    }

    // GET api/fixture/partido/5
    [HttpGet("partido/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPartido(int id)
    {
        var partido = await _fixtureService.GetPartidoByIdAsync(id);

        if (partido is null)
            return NotFound(new { mensaje = "Partido no encontrado." });

        return Ok(partido);
    }

    // POST api/fixture/generar
    [HttpPost("generar")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> GenerarAutomatico(GenerarFixtureRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var fixture = await _fixtureService.GenerarFixtureAutomaticoAsync(request, organizadorId);
            return Ok(fixture);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // POST api/fixture/partido
    [HttpPost("partido")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> CrearPartido(CrearPartidoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var partido = await _fixtureService.CrearPartidoAsync(request, organizadorId);
            return CreatedAtAction(nameof(GetPartido), new { id = partido.Id }, partido);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PUT api/fixture/partido/5
    [HttpPut("partido/{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> EditarPartido(int id, EditarPartidoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var partido = await _fixtureService.EditarPartidoAsync(id, request, organizadorId);
            return Ok(partido);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/fixture/partido/5
    [HttpDelete("partido/{id}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> EliminarPartido(int id)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _fixtureService.EliminarPartidoAsync(id, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PATCH api/fixture/partido/5/resultado
    [HttpPatch("partido/{id}/resultado")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> CargarResultado(int id, CargarResultadoRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var partido = await _fixtureService.CargarResultadoAsync(id, request, organizadorId);
            return Ok(partido);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // PATCH api/fixture/partido/5/estado
    [HttpPatch("partido/{id}/estado")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> CambiarEstado(int id, [FromBody] string nuevoEstado)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var partido = await _fixtureService.CambiarEstadoPartidoAsync(id, nuevoEstado, organizadorId);
            return Ok(partido);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}