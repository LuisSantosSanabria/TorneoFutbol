using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Jugadores;

namespace TorneoFutbol.Core.Interfaces
{
    public interface IJugadorService
    {
        Task<IEnumerable<JugadorResponse>> GetByEquipoAsync(int equipoId);
        Task<JugadorResponse?> GetByIdAsync(int id);
        Task<JugadorResponse> AltaAsync(AltaJugadorRequest request, int organizadorId);
        Task<JugadorResponse> EditarAsync(int id, EditarJugadorRequest request, int organizadorId);
        Task BajaAsync(int id, int organizadorId);
        Task<JugadorResponse> TransferirAsync(int id, TransferirJugadorRequest request, int organizadorId);
    }
}
