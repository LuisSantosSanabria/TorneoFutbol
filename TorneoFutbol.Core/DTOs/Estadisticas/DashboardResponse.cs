namespace TorneoFutbol.Core.DTOs.Estadisticas;

public class DashboardResponse
{
    public int TotalEquipos { get; set; }
    public int TotalJugadores { get; set; }
    public int TotalPartidos { get; set; }
    public int PartidosJugados { get; set; }
    public int PartidosPendientes { get; set; }
    public int TotalGoles { get; set; }
    public List<ProximoPartidoDto> ProximosPartidos { get; set; } = new();
    public List<UltimoResultadoDto> UltimosResultados { get; set; } = new();
    public List<EquipoGoleadorDto> EquiposMasGoleadores { get; set; } = new();
    public List<GoleadorResponse> TopGoleadores { get; set; } = new();
    public List<EquipoVallaMenosVencidaDto> VallasMenosVencidas { get; set; } = new();
}

public class ProximoPartidoDto
{
    public int PartidoId { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public string? Cancha { get; set; }
    public int FechaNumero { get; set; }
}

public class UltimoResultadoDto
{
    public int PartidoId { get; set; }
    public string EquipoLocal { get; set; } = string.Empty;
    public string EquipoVisitante { get; set; } = string.Empty;
    public int GolesLocal { get; set; }
    public int GolesVisitante { get; set; }
    public DateTime FechaHora { get; set; }
}

public class EquipoGoleadorDto
{
    public int EquipoId { get; set; }
    public string NombreEquipo { get; set; } = string.Empty;
    public int TotalGoles { get; set; }
}

public class EquipoVallaMenosVencidaDto
{
    public int EquipoId { get; set; }
    public string NombreEquipo { get; set; } = string.Empty;
    public int GolesRecibidos { get; set; }
    public int PartidosJugados { get; set; }
}