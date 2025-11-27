using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;

namespace Sconce.PL.Areas.Frontend
{
    [Route("api/[controller]")]
    [ApiController]
    public class DbController : ControllerBase
    {
        private readonly IDbService _dbService;

        public DbController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUserByEmail([FromQuery] string email)
        {
            bool isDeleted = await _dbService.DeleteUserByEmail(email);

            if (isDeleted)
                return Ok(new { Message = $"User with email {email} deleted successfully." });
            
            return NotFound(new { Message = $"User with email {email} not found." });
        }
    }
}
