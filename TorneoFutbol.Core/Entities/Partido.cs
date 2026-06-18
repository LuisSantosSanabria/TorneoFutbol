using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    public class Partido
    {
        public int Id { get; set; }
        public int FechaNumero { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Cancha { get; set; }
        public string Estado { get; set; } = "Pendiente"; // Pendiente | EnJuego | Finalizado | Suspendido
        public int? GolesLocal { get; set; }
        public int? GolesVisitante { get; set; }
        public int TorneoId { get; set; }
        public int EquipoLocalId { get; set; }
        public int EquipoVisitanteId { get; set; }

        // Navegación
        public Torneo Torneo { get; set; } = null!;
        public Equipo EquipoLocal { get; set; } = null!;
        public Equipo EquipoVisitante { get; set; } = null!;
        public ICollection<Gol> Goles { get; set; } = new List<Gol>();
        public ICollection<Tarjeta> Tarjetas { get; set; } = new List<Tarjeta>();
    }
}
