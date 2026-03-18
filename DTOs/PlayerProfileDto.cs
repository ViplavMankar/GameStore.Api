using System;

namespace GameStore.Api.DTOs;

public class PlayerProfileDto
{
    public string Username { get; set; } = string.Empty;

    public int TotalSessions { get; set; }
    public int TotalGamesPlayed { get; set; }
    public int TotalPlaytimeMinutes { get; set; }

    public string? FavoriteGame { get; set; }

    public List<RecentGameDto> RecentGames { get; set; } = new();
}