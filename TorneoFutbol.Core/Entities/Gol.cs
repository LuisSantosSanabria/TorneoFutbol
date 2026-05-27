using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    internal class Gol
    {
        public int Id { get; set; }
        public int Minuto { get; set; }
        public bool EsAutogol { get; set; } = false;
        public int PartidoId { get; set; }
        public int JugadorId { get; set; }
        public int EquipoId { get; set; }

        // Navegación
        public Partido Partido { get; set; } = null!;
        public Jugador Jugador { get; set; } = null!;
        public Equipo Equipo { get; set; } = null!;
    }
}
