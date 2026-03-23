using System;

namespace GameStore.Api.DTOs;

public class RecentGameDto
{
    public string GameTitle { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public DateTime PlayedAt { get; set; }
}