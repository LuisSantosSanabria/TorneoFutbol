using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Tarjetas
{
    public class RegistrarTarjetaRequest
    {
        public int JugadorId { get; set; }
        public int EquipoId { get; set; }
        public string Tipo { get; set; } = string.Empty; // Amarilla | Roja
        public int Minuto { get; set; }
    }
}
