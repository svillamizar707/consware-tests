using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TravelRequests.Application.DTOs;
using TravelRequests.Application.Interfaces;

namespace TravelRequests.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TravelRequestsController : ControllerBase
    {
        private readonly ITravelRequestService _service;

        public TravelRequestsController(ITravelRequestService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateTravelRequestDto dto)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var id = await _service.CreateAsync(dto, userId);
            return Ok(new { message = "Solicitud creada", id });
        }

        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var dto = await _service.GetByUserAsync(userId);
            return Ok(dto);
        }

        [HttpGet]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> GetAll()
        {
            var dto = await _service.GetAllAsync();
            return Ok(dto);
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string status)
        {
            await _service.ChangeStatusAsync(id, status);
            return Ok(new { message = "Estado actualizado." });
        }
    }
}
