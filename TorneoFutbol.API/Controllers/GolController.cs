using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.DTOs.Goles;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GolController : ControllerBase
{
    private readonly IGolService _golService;

    public GolController(IGolService golService)
    {
        _golService = golService;
    }

    private int GetUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(claim!);
    }

    // GET api/gol/partido/1
    [HttpGet("partido/{partidoId}")]
    public async Task<IActionResult> GetByPartido(int partidoId)
    {
        var goles = await _golService.GetByPartidoAsync(partidoId);
        return Ok(goles);
    }

    // POST api/gol/partido/1
    [HttpPost("partido/{partidoId}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Registrar(int partidoId, RegistrarGolRequest request)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            var gol = await _golService.RegistrarAsync(partidoId, request, organizadorId);
            return Ok(gol);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // DELETE api/gol/5
    [HttpDelete("{golId}")]
    [Authorize(Roles = "Organizador,Administrador")]
    public async Task<IActionResult> Anular(int golId)
    {
        try
        {
            var organizadorId = GetUsuarioId();
            await _golService.AnularAsync(golId, organizadorId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}