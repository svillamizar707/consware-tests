using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelRequests.Application.Interfaces;

namespace TravelRequests.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Authorize(Roles = "Aprobador")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _repo.GetAllAsync();
            var dto = users.Select(u => new { u.Id, u.Name, u.Email, u.Role });
            return Ok(dto);
        }
    }
}
