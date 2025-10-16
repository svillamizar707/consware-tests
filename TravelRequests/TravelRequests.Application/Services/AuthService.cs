using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Entities;
using TravelRequests.Application.Exceptions;

namespace TravelRequests.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IEmailService? _emailService;

        public AuthService(IUserRepository userRepository, IConfiguration config, IEmailService? emailService = null)
        {
            _userRepository = userRepository;
            _config = config;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> Register(UserRegisterDto dto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new AppException("El correo ya está registrado.");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return GenerateJwt(user);
        }

        public async Task<AuthResponseDto?> Login(UserLoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new AppException("Credenciales inválidas.");

            return GenerateJwt(user);
        }

        private AuthResponseDto GenerateJwt(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Role = user.Role,
                Email = user.Email,
                Name = user.Name
            };
        }

        // Genera y asigna un código de reseteo, retorna el código (simulando envío por correo)
        public async Task<string> ForgotPassword(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new AppException("Usuario no encontrado.");

            // Generar código (puede ser GUID o código corto)
            var code = Guid.NewGuid().ToString("N");
            user.ResetCode = code;
            user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(5);

            await _userRepository.SaveChangesAsync();

            // Enviar por correo si hay servicio
            if (_emailService != null)
            {
                await _emailService.SendAsync(email, "Password reset code", $"Your reset code: {code}");
            }

            // En un escenario real se enviaría por correo. Aquí retornamos el código.
            return code;
        }

        // Resetea la contraseña usando el código
        public async Task ResetPassword(string email, string code, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new AppException("Usuario no encontrado.");

            if (user.ResetCode == null || user.ResetCodeExpiry == null)
                throw new AppException("No hay una solicitud de reseteo activa.");

            if (user.ResetCodeExpiry < DateTime.UtcNow)
                throw new AppException("El código ha expirado.");

            if (!string.Equals(user.ResetCode, code, StringComparison.Ordinal))
                throw new AppException("Código inválido.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.ResetCode = null;
            user.ResetCodeExpiry = null;

            await _userRepository.SaveChangesAsync();
        }

        // Change password when authenticated
        public async Task ChangePassword(int userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                throw new AppException("Contraseña actual incorrecta.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.SaveChangesAsync();
        }
    }
}
