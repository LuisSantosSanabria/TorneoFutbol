using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Estadisticas;

namespace TorneoFutbol.Core.Interfaces
{
    public interface IEstadisticaService
    {
        Task<IEnumerable<PosicionResponse>> GetTablaPosicionesAsync(int torneoId);
        Task<IEnumerable<GoleadorResponse>> GetTablaGoleadoresAsync(int torneoId);
        Task<DashboardResponse> GetDashboardAsync(int torneoId);
    }
}
