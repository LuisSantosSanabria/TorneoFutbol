using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Equipos;

namespace TorneoFutbol.Core.Interfaces
{
    public interface IEquipoService
    {
        Task<IEnumerable<EquipoResponse>> GetByTorneoAsync(int torneoId);
        Task<EquipoResponse?> GetByIdAsync(int id);
        Task<EquipoResponse> CrearAsync(CrearEquipoRequest request, int organizadorId);
        Task<EquipoResponse> EditarAsync(int id, EditarEquipoRequest request, int organizadorId);
        Task EliminarAsync(int id, int organizadorId);
    }
}
