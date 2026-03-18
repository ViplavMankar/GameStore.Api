using System;

namespace GameStore.Api.DTOs;

public class GamePlaytimeDto
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double TotalMinutes { get; set; }
}