using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Fixture
{
    public class GenerarFixtureRequest
    {
        public int TorneoId { get; set; }
        public DateTime FechaInicioPrimerFecha { get; set; }
        public int DiasEntrePartidos { get; set; } = 7;
        public string? Cancha { get; set; }
    }
}
