using System;
using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class GameRatingService : IGameRatingService
{
    private readonly GameStoreDbContext _context;

    public GameRatingService(GameStoreDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Add or update a user's rating for a game.
    /// </summary>
    public async Task<bool> RateGameAsync(Guid gameId, Guid userId, int rating)
    {
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating to be between 1 and 5");
        }
        var existingRating = await _context.GameRatings.FirstOrDefaultAsync(r => r.GameId == gameId && r.UserId == userId);
        if (existingRating == null)
        {
            var newRating = new GameRating
            {
                GameId = gameId,
                UserId = userId,
                Rating = rating,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            _context.GameRatings.Add(newRating);
        }
        else
        {
            existingRating.Rating = rating;
            existingRating.UpdatedAt = DateTime.UtcNow;
            _context.GameRatings.Update(existingRating);
        }
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get the rating a specific user gave a specific game.
    /// </summary>
    public async Task<int?> GetUserRatingAsync(Guid gameId, Guid userId)
    {
        var rating = await _context.GameRatings
                                    .Where(r => r.GameId == gameId && r.UserId == userId)
                                    .Select(r => (int?)r.Rating)
                                    .FirstOrDefaultAsync();
        return rating;
    }

    /// <summary>
    /// Get the average rating (out of 5) and total votes for a game.
    /// </summary>
    public async Task<(double averageRating, int totalVotes)> GetGameRatingStatsAsync(Guid gameId)
    {
        var query = _context.GameRatings.Where(r => r.GameId == gameId);
        var totalVotes = await query.CountAsync();

        if (totalVotes == 0)
        {
            return (0, 0);
        }
        var average = await query.AverageAsync(r => r.Rating);
        return (Math.Round(average, 2), totalVotes);
    }
}
