using System;

namespace GameStore.Api.Models;

public class UserStats
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    // Core metrics
    public int GamesPlayed { get; set; } = 0;

    public int SessionsCount { get; set; } = 0;

    public int TotalSecondsPlayed { get; set; } = 0;

    public int TotalPurchases { get; set; } = 0;

    // Optional (nice for UI / tracking)
    public DateTime LastPlayedAt { get; set; }

    // 🔥 XP SYSTEM
    public int TotalXP { get; set; } = 0;
}