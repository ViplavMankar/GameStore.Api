using GameStore.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ITrendingGameService _service;

        public StatisticsController(ITrendingGameService service)
        {
            _service = service;
        }
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingGames()
        {
            var trendingGames = await _service.GetTrendingGamesAsync();
            return Ok(trendingGames);
        }
        [HttpGet("total-playtime")]
        public async Task<IActionResult> GetTotalPlaytimePerGame()
        {
            var result = await _service.GetTotalPlaytimePerGameAsync();
            return Ok(result);
        }
        [HttpGet("daily-active-players")]
        public async Task<IActionResult> GetDailyActivePlayers([FromQuery] int days = 7)
        {
            var data = await _service.GetDailyActivePlayersAsync(days);

            return Ok(data);
        }
    }
}
