using System;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Data;
using GameStore.Api.Models;
namespace GameStore.Api.Services;

public class UserStatisticsService : IUserStatisticsService
{
    private readonly GameStoreDbContext _context;

    public UserStatisticsService(GameStoreDbContext context)
    {
        _context = context;
    }

    // 🔹 Get or create stats (CORE method)
    public async Task<UserStats> GetOrCreateAsync(Guid userId)
    {
        var stats = await _context.UserStatistics
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (stats == null)
        {
            stats = new UserStats
            {
                UserId = userId,
                GamesPlayed = 0,
                SessionsCount = 0,
                TotalSecondsPlayed = 0,
                TotalPurchases = 0,
                LastPlayedAt = DateTime.UtcNow
            };

            _context.UserStatistics.Add(stats);
            await _context.SaveChangesAsync();
        }

        return stats;
    }

    // 🎮 Called when session ends
    public async Task UpdateAfterSessionAsync(Guid userId, Guid gameId, int secondsPlayed)
    {
        if (secondsPlayed <= 0)
            return;

        var stats = await GetOrCreateAsync(userId);

        stats.SessionsCount += 1;
        stats.TotalSecondsPlayed += secondsPlayed;
        stats.LastPlayedAt = DateTime.UtcNow;

        // 🔥 KEY FIX: Check if this game was already played
        var hasPlayedGameBefore = await _context.GameSessions
            .AnyAsync(s => s.UserId == userId && s.GameId == gameId && s.EndedAt != null);

        if (!hasPlayedGameBefore)
        {
            stats.GamesPlayed += 1;
        }

        await _context.SaveChangesAsync();
    }

    // 💳 Called when purchase happens (future use)
    public async Task UpdateAfterPurchaseAsync(Guid userId)
    {
        var stats = await GetOrCreateAsync(userId);

        stats.TotalPurchases += 1;

        await _context.SaveChangesAsync();
    }

    // 📊 Get stats (for UI / API)
    public async Task<UserStats?> GetStatsAsync(Guid userId)
    {
        return await _context.UserStatistics
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    // 🔄 Optional: Recalculate from scratch (VERY useful for debugging)
    public async Task RecalculateStatsAsync(Guid userId)
    {
        var sessions = await _context.GameSessions
            .Where(x => x.UserId == userId && x.EndedAt != null)
            .ToListAsync();

        var stats = await GetOrCreateAsync(userId);

        stats.SessionsCount = sessions.Count;

        stats.TotalSecondsPlayed = sessions
            .Sum(s => (int)(s.EndedAt.Value - s.StartedAt).TotalMinutes);

        stats.GamesPlayed = sessions
            .Select(s => s.GameId)
            .Distinct()
            .Count();

        await _context.SaveChangesAsync();
    }
}
