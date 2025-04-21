using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Data;

namespace AuthService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly GameStoreDbContext _dbContext;

        public HealthController(GameStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("health")]
        public async Task<IActionResult> Health()
        {
            try
            {
                // Executes a simple query to check DB connectivity
                await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");

                return Ok(new
                {
                    status = "Healthy",
                    database = "Connected"
                });
            }
            catch
            {
                return Problem("Database connection failed", statusCode: 500);
            }
        }
    }
}
