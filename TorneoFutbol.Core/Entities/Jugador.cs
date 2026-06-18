using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    public class Jugador
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int NumeroCamiseta { get; set; }
        public string Posicion { get; set; } = string.Empty; // ARQ | DEF | MED | DEL
        public string FotoUrl { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public int EquipoId { get; set; }

        // Navegación
        public Equipo Equipo { get; set; } = null!;
        public ICollection<Gol> Goles { get; set; } = new List<Gol>();
        public ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
    }
}
