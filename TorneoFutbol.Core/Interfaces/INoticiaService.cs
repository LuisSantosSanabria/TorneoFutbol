using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Noticias;

namespace TorneoFutbol.Core.Interfaces
{
    public interface INoticiaService
    {
        Task<IEnumerable<NoticiaResponse>> GetByTorneoAsync(int torneoId);
        Task<NoticiaResponse?> GetByIdAsync(int id);
        Task<NoticiaResponse> CrearAsync(CrearNoticiaRequest request, int organizadorId);
        Task<NoticiaResponse> EditarAsync(int id, EditarNoticiaRequest request, int organizadorId);
        Task EliminarAsync(int id, int organizadorId);
    }
}
