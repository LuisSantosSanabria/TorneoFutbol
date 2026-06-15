using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Fixture
{
    public class CrearPartidoRequest
    {
        public int TorneoId { get; set; }
        public int FechaNumero { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Cancha { get; set; }
        public int EquipoLocalId { get; set; }
        public int EquipoVisitanteId { get; set; }
    }
}
