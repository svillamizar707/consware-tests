using TravelRequests.Application.DTOs;
using TravelRequests.Application.Interfaces;
using TravelRequests.Domain.Entities;

namespace TravelRequests.Application.Services
{
    public class TravelRequestService : ITravelRequestService
    {
        private readonly ITravelRequestRepository _repo;

        public TravelRequestService(ITravelRequestRepository repo)
        {
            _repo = repo;
        }

        public async Task<int> CreateAsync(CreateTravelRequestDto dto, int userId)
        {
            // validations should be in FluentValidation, but double-check
            if (dto.ReturnDate <= dto.DepartureDate)
                throw new Application.Exceptions.AppException("La fecha de regreso debe ser posterior a la fecha de ida.");
            if (string.Equals(dto.OriginCity?.Trim(), dto.DestinationCity?.Trim(), StringComparison.OrdinalIgnoreCase))
                throw new Application.Exceptions.AppException("La ciudad de origen y destino no pueden ser la misma.");

            var tr = new TravelRequest
            {
                OriginCity = dto.OriginCity,
                DestinationCity = dto.DestinationCity,
                DepartureDate = dto.DepartureDate,
                ReturnDate = dto.ReturnDate,
                Justification = dto.Justification,
                Status = "Pendiente",
                UserId = userId
            };

            await _repo.AddAsync(tr);
            await _repo.SaveChangesAsync();

            return tr.Id;
        }

        public async Task<IEnumerable<TravelRequestDto>> GetByUserAsync(int userId)
        {
            var list = await _repo.GetByUserIdAsync(userId);
            return list.Select(r => new TravelRequestDto
            {
                Id = r.Id,
                OriginCity = r.OriginCity,
                DestinationCity = r.DestinationCity,
                DepartureDate = r.DepartureDate,
                ReturnDate = r.ReturnDate,
                Justification = r.Justification,
                Status = r.Status,
                UserId = r.UserId,
                UserName = r.User?.Name ?? string.Empty
            });
        }

        public async Task<IEnumerable<TravelRequestDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(r => new TravelRequestDto
            {
                Id = r.Id,
                OriginCity = r.OriginCity,
                DestinationCity = r.DestinationCity,
                DepartureDate = r.DepartureDate,
                ReturnDate = r.ReturnDate,
                Justification = r.Justification,
                Status = r.Status,
                UserId = r.UserId,
                UserName = r.User?.Name ?? string.Empty
            });
        }

        public async Task ChangeStatusAsync(int id, string status)
        {
            if (status != "Aprobada" && status != "Rechazada")
                throw new Application.Exceptions.AppException("Estado inválido.");

            var tr = await _repo.GetByIdAsync(id);
            if (tr == null) throw new Application.Exceptions.NotFoundException("Solicitud no encontrada.");

            tr.Status = status;
            await _repo.UpdateAsync(tr);
            await _repo.SaveChangesAsync();
        }
    }
}
