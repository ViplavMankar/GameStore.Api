using System;

namespace GameStore.Api.DTOs;

public class TrendingGameDto
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SessionCount { get; set; }
}