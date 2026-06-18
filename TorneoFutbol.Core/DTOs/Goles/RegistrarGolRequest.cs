using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Goles
{
    public class RegistrarGolRequest
    {
        public int JugadorId { get; set; }
        public int EquipoId { get; set; }
        public int Minuto { get; set; }
        public bool EsAutogol { get; set; } = false;
    }
}
