using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Equipos
{
    public class EditarEquipoRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string EscudoUrl { get; set; } = string.Empty;
    }
}
