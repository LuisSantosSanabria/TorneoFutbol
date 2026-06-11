using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TorneoFutbol.Core.DTOs.Auth;
using TorneoFutbol.Core.Entities;
using TorneoFutbol.Core.Interfaces;
using TorneoFutbol.Infrastructure.Data;
using BCrypt.Net;

namespace TorneoFutbol.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly TorneoFutbolDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(TorneoFutbolDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    // Registro
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Verificar si el email ya existe
        var emailExiste = await _context.Usuarios
            .AnyAsync(u => u.Email == request.Email);

        if (emailExiste)
            throw new Exception("El email ya está registrado.");

        // Crear el usuario con la contraseña encriptada
        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Rol = "Usuario" // Por defecto siempre es Usuario
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Generar y devolver el token
        var token = GenerarToken(usuario);

        return new AuthResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Token = token
        };
    }

    // Login
    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        // Buscar el usuario por email
        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        // Verificar que existe y que la contraseña es correcta
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Password, usuario.PasswordHash))
            throw new Exception("Email o contraseña incorrectos.");

        var token = GenerarToken(usuario);

        return new AuthResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol,
            Token = token
        };
    }

    // Generar Token JWT
    private string GenerarToken(Usuario usuario)
    {
        // Claims: datos que viajan dentro del token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Nombre),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        // Clave secreta para firmar el token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Armar el token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["Jwt:ExpiresMinutes"]!)),
            signingCredentials: credenciales
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}