using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    internal class Equipo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string EscudoUrl { get; set; } = string.Empty;
        public int TorneoId { get; set; }

        // Navegación
        public Torneo Torneo { get; set; } = null!;
        public ICollection<Jugador> Jugadores { get; set; } = new List<Jugador>();
    }
}
