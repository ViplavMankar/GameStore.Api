using System;

namespace GameStore.Api.Models;

public class Achievement
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    // Example: "Sessions", "PlayTime", "GamesPlayed"
    public string ConditionType { get; set; } = string.Empty;

    public int TargetValue { get; set; }

    // Navigation
    public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
}