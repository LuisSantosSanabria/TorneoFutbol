using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    public class Torneo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; } = "Planificado"; // Planificado | EnCurso | Finalizado
        public int OrganizadorId { get; set; }

        // Navegación
        public Usuario Organizador { get; set; } = null!;
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
        public ICollection<Partido> Partidos { get; set; } = new List<Partido>();
    }
}
