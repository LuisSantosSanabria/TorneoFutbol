using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Goles;

namespace TorneoFutbol.Core.Interfaces
{
    public interface IGolService
    {
        Task<IEnumerable<GolResponse>> GetByPartidoAsync(int partidoId);
        Task<GolResponse> RegistrarAsync(int partidoId, RegistrarGolRequest request, int organizadorId);
        Task AnularAsync(int golId, int organizadorId);
    }
}
