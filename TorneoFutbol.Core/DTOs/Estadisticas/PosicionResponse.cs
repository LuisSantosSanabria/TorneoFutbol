using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TorneoFutbol.Core.DTOs.Estadisticas
{
    public class PosicionResponse
    {
        public int Posicion { get; set; }
        public int EquipoId { get; set; }
        public string NombreEquipo { get; set; } = string.Empty;
        public string EscudoUrl { get; set; } = string.Empty;
        public int PJ { get; set; }  // Partidos Jugados
        public int PG { get; set; }  // Partidos Ganados
        public int PE { get; set; }  // Partidos Empatados
        public int PP { get; set; }  // Partidos Perdidos
        public int GF { get; set; }  // Goles a Favor
        public int GC { get; set; }  // Goles en Contra
        public int DG { get; set; }  // Diferencia de Goles
        public int PTS { get; set; } // Puntos
    }
}
