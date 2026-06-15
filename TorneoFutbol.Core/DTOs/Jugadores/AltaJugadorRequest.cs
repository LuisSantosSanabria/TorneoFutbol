using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Jugadores
{
    public class AltaJugadorRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public int NumeroCamiseta { get; set; }
        public string Posicion { get; set; } = string.Empty; // ARQ | DEF | MED | DEL
        public int EquipoId { get; set; }
    }
}
