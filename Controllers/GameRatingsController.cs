using System.Security.Claims;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Controllers
{
    [ApiController]
    [Route("api/games/{gameId}/ratings")]
    public class GameRatingsController : ControllerBase
    {
        private readonly IGameRatingService _ratingService;

        public GameRatingsController(IGameRatingService ratingService)
        {
            _ratingService = ratingService;
        }
        /// <summary>
        /// Get average rating, total votes, and (if logged in) the user's rating for this game.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRatingStats(Guid gameId)
        {
            var (averageRating, totalVotes) = await _ratingService.GetGameRatingStatsAsync(gameId);

            int? userRating = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                userRating = await _ratingService.GetUserRatingAsync(gameId, userId);
            }
            return Ok(
                new
                {
                    GameId = gameId,
                    AverageRating = averageRating,
                    TotalVotes = totalVotes,
                    UserRating = userRating
                }
            );
        }

        /// <summary>
        /// Rate or update rating for a game (1–5 stars).
        /// Requires authentication.
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RateGame(Guid gameId, [FromBody] int rating)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _ratingService.RateGameAsync(gameId, userId, rating);

            var (averageRating, totalVotes) = await _ratingService.GetGameRatingStatsAsync(gameId);

            return Ok(
                new
                {
                    GameId = gameId,
                    AverageRating = averageRating,
                    TotalVotes = totalVotes,
                    UserRating = rating
                }
            );
        }

    }
}
