using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Jugadores
{
    public class EditarJugadorRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public int NumeroCamiseta { get; set; }
        public string Posicion { get; set; } = string.Empty;
    }
}
