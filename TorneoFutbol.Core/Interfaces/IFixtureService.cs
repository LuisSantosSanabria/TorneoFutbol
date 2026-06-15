using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Fixture;

namespace TorneoFutbol.Core.Interfaces
{
    public interface IFixtureService
    {
        // Consultas
        Task<IEnumerable<FechaResponse>> GetFixtureByTorneoAsync(int torneoId);
        Task<IEnumerable<PartidoResponse>> GetByFechaAsync(int torneoId, int fechaNumero);
        Task<PartidoResponse?> GetPartidoByIdAsync(int id);

        // Fixture automático
        Task<IEnumerable<FechaResponse>> GenerarFixtureAutomaticoAsync(GenerarFixtureRequest request, int organizadorId);

        // Fixture manual
        Task<PartidoResponse> CrearPartidoAsync(CrearPartidoRequest request, int organizadorId);
        Task<PartidoResponse> EditarPartidoAsync(int id, EditarPartidoRequest request, int organizadorId);
        Task EliminarPartidoAsync(int id, int organizadorId);

        // Resultados
        Task<PartidoResponse> CargarResultadoAsync(int id, CargarResultadoRequest request, int organizadorId);
        Task<PartidoResponse> CambiarEstadoPartidoAsync(int id, string nuevoEstado, int organizadorId);
    }
}
