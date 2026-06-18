using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Goles
{
    public class GolResponse
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        public int JugadorId { get; set; }
        public string NombreJugador { get; set; } = string.Empty;
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public int Minuto { get; set; }
        public bool EsAutogol { get; set; }
    }
}
