using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Estadisticas
{
    public class GoleadorResponse
    {
        public int Posicion { get; set; }
        public int JugadorId { get; set; }
        public string NombreJugador { get; set; } = string.Empty;
        public string NombreEquipo { get; set; } = string.Empty;
        public string Posicion_Juego { get; set; } = string.Empty;
        public int Goles { get; set; }
        public int GolesAutogol { get; set; }
    }
}
