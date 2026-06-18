using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Tarjetas;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarjetaController : ControllerBase
{
    private readonly ITarjetaService _tarjetaService;

    public TarjetaController(ITarjetaService tarjetaService)
    {
        _tarjetaService = tarjetaService;
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/tarjeta/partido/1
    [HttpGet("partido/{partidoId}")]
    public async Task<IActionResult> GetByPartido(int partidoId)
    {
        var tarjetas = await _tarjetaService.GetByPartidoAsync(partidoId);
        return Ok(tarjetas);
    }

    // POST api/tarjeta/partido/1
    [HttpPost("partido/{partidoId}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Registrar(int partidoId, RegistrarTarjetaRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var tarjeta = await _tarjetaService.RegistrarAsync(partidoId, request, organizadorId);
            return Ok(tarjeta);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/tarjeta/5
    [HttpDelete("{tarjetaId}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Anular(int tarjetaId)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _tarjetaService.AnularAsync(tarjetaId, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}