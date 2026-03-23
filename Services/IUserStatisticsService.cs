using System;
using GameStore.Api.Models;

namespace GameStore.Api.Services;

public interface IUserStatisticsService
{
    Task<UserStats> GetOrCreateAsync(Guid userId);
    Task UpdateAfterSessionAsync(Guid userId, Guid gameId, int secondsPlayed);
    Task UpdateAfterPurchaseAsync(Guid userId);
    Task<UserStats?> GetStatsAsync(Guid userId);
    Task RecalculateStatsAsync(Guid userId);
}
