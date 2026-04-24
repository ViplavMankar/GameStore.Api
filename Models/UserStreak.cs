using System;

namespace GameStore.Api.Models;

public class UserStreak
{
    public Guid UserId { get; set; }

    public int CurrentStreak { get; set; }
    public int MaxStreak { get; set; }

    public DateTime LastPlayedDateUtc { get; set; }

    public DateTime UpdatedAtUtc { get; set; }
}
