using System;

namespace GameStore.Api.Models;

public class LeaderboardEntry
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string Username { get; set; }

    public int TotalPlayTimeSeconds { get; set; }

    public int Rank { get; set; }

    public DateTime LastUpdated { get; set; }
}