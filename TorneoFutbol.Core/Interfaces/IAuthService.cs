using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TorneoFutbol.Core.DTOs.Auth;

namespace TorneoFutbol.Core.Interfaces
{
    // interface por que no quiero acoplar mi controlador a una implementacion concreta, sino a una abstraccion,
    // asi puedo cambiar la implementacion sin afectar al controlador
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
