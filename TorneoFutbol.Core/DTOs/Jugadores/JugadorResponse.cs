using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Jugadores
{
    public class JugadorResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int NumeroCamiseta { get; set; }
        public string Posicion { get; set; } = string.Empty;
        public string FotoUrl { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
    }
}
