using System;
using System.Threading.Tasks;
using Moq;
using TravelRequests.Application.Interfaces;
using TravelRequests.Application.Services;
using TravelRequests.Application.DTOs;
using TravelRequests.Domain.Entities;
using Xunit;
using TravelRequests.Application.Exceptions;

namespace TravelRequests.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task Register_ShouldCreateUserAndReturnToken()
        {
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            // Use a 32+ character key for HS256
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var authService = new AuthService(userRepoMock.Object, configMock.Object, null);

            var dto = new UserRegisterDto
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "Password123!",
                Role = "Solicitante"
            };

            var result = await authService.Register(dto);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result.Token));
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldThrow()
        {
            var user = new User { Id = 1, Email = "a@b.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("secret"), Role = "Solicitante" };

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var authService = new AuthService(userRepoMock.Object, configMock.Object, null);

            var dto = new UserLoginDto { Email = user.Email, Password = "wrongpassword" };

            await Assert.ThrowsAsync<AppException>(async () => await authService.Login(dto));
        }

        [Fact]
        public async Task ForgotPassword_ShouldSaveCodeAndCallEmailService()
        {
            // Arrange
            var user = new User { Id = 10, Email = "user@example.com" };

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var emailMock = new Mock<IEmailService>();
            emailMock.Setup(e => e.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var service = new AuthService(userRepoMock.Object, configMock.Object, emailMock.Object);

            // Act
            var code = await service.ForgotPassword(user.Email);

            // Assert
            Assert.False(string.IsNullOrEmpty(code));
            Assert.Equal(code, user.ResetCode);
            Assert.NotNull(user.ResetCodeExpiry);
            Assert.True(user.ResetCodeExpiry > DateTime.UtcNow);
            emailMock.Verify(e => e.SendAsync(user.Email, It.IsAny<string>(), It.Is<string>(b => b.Contains(code))), Times.Once);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ResetPassword_WithValidCode_ShouldUpdatePassword()
        {
            // Arrange
            var user = new User { Id = 20, Email = "u2@example.com" };
            user.ResetCode = "abc123";
            user.ResetCodeExpiry = DateTime.UtcNow.AddMinutes(5);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("oldpass");

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var service = new AuthService(userRepoMock.Object, configMock.Object, null);

            // Act
            await service.ResetPassword(user.Email, "abc123", "MyNewP@ss1");

            // Assert
            Assert.True(BCrypt.Net.BCrypt.Verify("MyNewP@ss1", user.PasswordHash));
            Assert.Null(user.ResetCode);
            Assert.Null(user.ResetCodeExpiry);
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_WithCorrectOldPassword_ShouldUpdatePassword()
        {
            // Arrange
            var user = new User { Id = 30, Email = "u3@example.com" };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword("currentPwd");

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            userRepoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var service = new AuthService(userRepoMock.Object, configMock.Object, null);

            // Act
            await service.ChangePassword(user.Id, "currentPwd", "BrandNew1!");

            // Assert
            Assert.True(BCrypt.Net.BCrypt.Verify("BrandNew1!", user.PasswordHash));
            userRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Register_WhenEmailExists_ShouldThrow()
        {
            // Arrange
            var existing = new User { Id = 40, Email = "exists@example.com" };
            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock.Setup(r => r.GetByEmailAsync(existing.Email)).ReturnsAsync(existing);

            var configMock = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
            configMock.Setup(c => c["Jwt:Key"]).Returns("abcdefghijklmnopqrstuvwxyzABCDEF");
            configMock.Setup(c => c["Jwt:Issuer"]).Returns("issuer");
            configMock.Setup(c => c["Jwt:Audience"]).Returns("audience");

            var service = new AuthService(userRepoMock.Object, configMock.Object, null);

            var dto = new UserRegisterDto { Name = "X", Email = existing.Email, Password = "p", Role = "Solicitante" };

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await service.Register(dto));
        }
    }
}
