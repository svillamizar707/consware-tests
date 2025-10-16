using TravelRequests.Application.DTOs;

namespace TravelRequests.Application.Interfaces
{
    public interface ITravelRequestService
    {
        Task<int> CreateAsync(CreateTravelRequestDto dto, int userId);
        Task<IEnumerable<TravelRequestDto>> GetByUserAsync(int userId);
        Task<IEnumerable<TravelRequestDto>> GetAllAsync();
        Task ChangeStatusAsync(int id, string status);
    }
}
