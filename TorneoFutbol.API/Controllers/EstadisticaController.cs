using Microsoft.AspNetCore.Mvc;
using TorneoFutbol.Core.Interfaces;

namespace TorneoFutbol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EstadisticaController : ControllerBase
{
    private readonly IEstadisticaService _estadisticaService;

    public EstadisticaController(IEstadisticaService estadisticaService)
    {
        _estadisticaService = estadisticaService;
    }

    // GET api/estadistica/torneo/1/posiciones
    [HttpGet("torneo/{torneoId}/posiciones")]
    public async Task<IActionResult> GetTablaPosiciones(int torneoId)
    {
        try
        {
            var tabla = await _estadisticaService.GetTablaPosicionesAsync(torneoId);
            return Ok(tabla);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    // GET api/estadistica/torneo/1/goleadores
    [HttpGet("torneo/{torneoId}/goleadores")]
    public async Task<IActionResult> GetTablaGoleadores(int torneoId)
    {
        try
        {
            var tabla = await _estadisticaService.GetTablaGoleadoresAsync(torneoId);
            return Ok(tabla);
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}