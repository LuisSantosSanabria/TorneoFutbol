using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Equipos
{
    // DTO para devolver la información de un equipo, incluyendo el nombre del torneo y la cantidad de jugadores
    public class EquipoResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string EscudoUrl { get; set; } = string.Empty;
        public int TorneoId { get; set; }
        public string NombreTorneo { get; set; } = string.Empty;
        public int CantidadJugadores { get; set; }
    }
}
