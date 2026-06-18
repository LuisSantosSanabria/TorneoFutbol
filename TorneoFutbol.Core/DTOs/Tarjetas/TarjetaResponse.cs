using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Tarjetas
{
    public class TarjetaResponse
    {
        public int Id { get; set; }
        public int PartidoId { get; set; }
        public int JugadorId { get; set; }
        public string NombreJugador { get; set; } = string.Empty;
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Minuto { get; set; }
    }
}
