using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Fixture
{
    public class PartidoResponse
    {
        public int Id { get; set; }
        public int FechaNumero { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Cancha { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int TorneoId { get; set; }
        public string NombreTorneo { get; set; } = string.Empty;
        public int EquipoLocalId { get; set; }
        public string NombreEquipoLocal { get; set; } = string.Empty;
        public int EquipoVisitanteId { get; set; }
        public string NombreEquipoVisitante { get; set; } = string.Empty;
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public string Resultado { get; set; } = string.Empty; // "2 - 1" o "Pendiente"
    }
}
