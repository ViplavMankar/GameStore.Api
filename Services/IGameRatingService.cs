using System;

namespace GameStore.Api.Services;

public interface IGameRatingService
{
    Task<bool> RateGameAsync(Guid gameId, Guid userId, int rating);
    Task<int?> GetUserRatingAsync(Guid gameId, Guid userId);
    Task<(double averageRating, int totalVotes)> GetGameRatingStatsAsync(Guid gameId);
}
