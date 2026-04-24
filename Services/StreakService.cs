using System;
using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class StreakService
{
    private readonly GameStoreDbContext _db;

    public StreakService(GameStoreDbContext db)
    {
        _db = db;
    }

    public async Task<StreakResult> UpdateStreakAsync(Guid userId)
    {
        var today = DateTime.UtcNow.Date;

        var streak = await _db.UserStreaks
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (streak == null)
        {
            streak = new UserStreak
            {
                UserId = userId,
                CurrentStreak = 1,
                MaxStreak = 1,
                LastPlayedDateUtc = today,
                UpdatedAtUtc = DateTime.UtcNow
            };

            _db.UserStreaks.Add(streak);
        }
        else
        {
            var lastDate = streak.LastPlayedDateUtc.Date;

            if (lastDate == today)
            {
                // Already counted today → do nothing
                return new StreakResult(streak, false);
            }

            if (lastDate == today.AddDays(-1))
            {
                // Continue streak
                streak.CurrentStreak++;
            }
            else
            {
                // Missed day → reset
                streak.CurrentStreak = 1;
            }

            if (streak.CurrentStreak > streak.MaxStreak)
                streak.MaxStreak = streak.CurrentStreak;

            streak.LastPlayedDateUtc = today;
            streak.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return new StreakResult(streak, true);
    }

    public record StreakResult(UserStreak Streak, bool UpdatedToday);
}
