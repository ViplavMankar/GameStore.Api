using System;

namespace GameStore.Api.Models;

public class UserAchievement
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid AchievementId { get; set; }

    public DateTime UnlockedAt { get; set; }

    // Navigation
    public Achievement Achievement { get; set; } = null!;
}