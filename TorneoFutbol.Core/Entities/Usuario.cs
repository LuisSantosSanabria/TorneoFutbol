using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.Entities
{
    internal class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Rol { get; set; } = "Usuario"; // Usuario | Organizador | Administrador

        // Navegación
        public ICollection<Torneo> Torneos { get; set; } = new List<Torneo>();
    }
}
