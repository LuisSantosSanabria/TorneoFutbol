using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Torneos;

namespace TorneoFutbol.Core.Interfaces
{
    public interface ITorneoService
    {
        Task<IEnumerable<TorneoResponse>> GetAllAsync();
        Task<TorneoResponse?> GetByIdAsync(int id);
        Task<TorneoResponse> CrearAsync(CrearTorneoRequest request, int organizadorId);
        Task<TorneoResponse> EditarAsync(int id, EditarTorneoRequest request, int organizadorId);
        Task EliminarAsync(int id, int organizadorId);
        Task<TorneoResponse> CambiarEstadoAsync(int id, string nuevoEstado, int organizadorId);
    }
}
