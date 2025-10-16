using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Interfaces;
using TravelRequests.Application.Services;
using TravelRequests.Domain.Entities;
using Xunit;
using TravelRequests.Application.Exceptions;

namespace TravelRequests.Tests
{
    public class TravelRequestServiceTests
    {
        [Fact]
        public async Task CreateAsync_ValidDto_ShouldCreateAndReturnId()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            TravelRequest captured = null!;
            repoMock.Setup(r => r.AddAsync(It.IsAny<TravelRequest>()))
                .Callback<TravelRequest>(t => { captured = t; captured.Id = 42; })
                .Returns(Task.CompletedTask);
            repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var service = new TravelRequestService(repoMock.Object);

            var dto = new CreateTravelRequestDto
            {
                OriginCity = "A",
                DestinationCity = "B",
                DepartureDate = System.DateTime.UtcNow.AddDays(1),
                ReturnDate = System.DateTime.UtcNow.AddDays(2),
                Justification = "Reason"
            };

            // Act
            var id = await service.CreateAsync(dto, userId: 7);

            // Assert
            Assert.Equal(42, id);
            Assert.Equal(7, captured.UserId);
            Assert.Equal("Pendiente", captured.Status);
            repoMock.Verify(r => r.AddAsync(It.IsAny<TravelRequest>()), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_InvalidDates_ShouldThrow()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            var service = new TravelRequestService(repoMock.Object);

            var dto = new CreateTravelRequestDto
            {
                OriginCity = "A",
                DestinationCity = "A",
                DepartureDate = System.DateTime.UtcNow.AddDays(5),
                ReturnDate = System.DateTime.UtcNow.AddDays(1),
                Justification = "x"
            };

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await service.CreateAsync(dto, 1));
        }

        [Fact]
        public async Task GetByUserAsync_ShouldReturnDtos()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            var tr = new TravelRequest
            {
                Id = 1,
                OriginCity = "X",
                DestinationCity = "Y",
                DepartureDate = System.DateTime.UtcNow,
                ReturnDate = System.DateTime.UtcNow.AddDays(1),
                Justification = "J",
                Status = "Pendiente",
                UserId = 5,
                User = new User { Id = 5, Name = "User5" }
            };
            repoMock.Setup(r => r.GetByUserIdAsync(5)).ReturnsAsync(new List<TravelRequest> { tr });

            var service = new TravelRequestService(repoMock.Object);

            // Act
            var result = (await service.GetByUserAsync(5)).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(tr.Id, result[0].Id);
            Assert.Equal(tr.UserId, result[0].UserId);
            Assert.Equal("User5", result[0].UserName);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnDtos()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            var tr = new TravelRequest
            {
                Id = 2,
                OriginCity = "O",
                DestinationCity = "D",
                DepartureDate = System.DateTime.UtcNow,
                ReturnDate = System.DateTime.UtcNow.AddDays(3),
                Justification = "J2",
                Status = "Pendiente",
                UserId = 6,
                User = new User { Id = 6, Name = "User6" }
            };
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TravelRequest> { tr });

            var service = new TravelRequestService(repoMock.Object);

            // Act
            var result = (await service.GetAllAsync()).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(tr.Id, result[0].Id);
            Assert.Equal("User6", result[0].UserName);
        }

        [Fact]
        public async Task ChangeStatusAsync_Valid_ShouldUpdate()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            var tr = new TravelRequest { Id = 3, Status = "Pendiente" };
            repoMock.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(tr);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<TravelRequest>())).Returns(Task.CompletedTask);
            repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            var service = new TravelRequestService(repoMock.Object);

            // Act
            await service.ChangeStatusAsync(3, "Aprobada");

            // Assert
            Assert.Equal("Aprobada", tr.Status);
            repoMock.Verify(r => r.UpdateAsync(It.Is<TravelRequest>(x => x.Id == 3 && x.Status == "Aprobada")), Times.Once);
            repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ChangeStatusAsync_InvalidStatus_ShouldThrow()
        {
            // Arrange
            var repoMock = new Mock<ITravelRequestRepository>();
            var service = new TravelRequestService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AppException>(async () => await service.ChangeStatusAsync(1, "Invalid"));
        }
    }
}
