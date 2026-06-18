using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Tarjetas;

namespace TorneoFutbol.Core.Interfaces
{
    public interface ITarjetaService
    {
        Task<IEnumerable<TarjetaResponse>> GetByPartidoAsync(int partidoId);
        Task<TarjetaResponse> RegistrarAsync(int partidoId, RegistrarTarjetaRequest request, int organizadorId);
        Task AnularAsync(int tarjetaId, int organizadorId);
    }
}
