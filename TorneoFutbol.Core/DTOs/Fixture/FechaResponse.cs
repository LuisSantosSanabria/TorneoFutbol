using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Fixture
{
    public class FechaResponse
    {
        public int FechaNumero { get; set; }
        public List<PartidoResponse> Partidos { get; set; } = new();
    }
}
