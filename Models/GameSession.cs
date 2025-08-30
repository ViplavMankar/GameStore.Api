using System;

namespace GameStore.Api.Models;

public class GameSession
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // Who is playing
    public Guid UserId { get; set; }

    // Which game is being played
    public Guid GameId { get; set; }

    // When it started and ended
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    // Optional: Track duration in seconds (can also compute on query)
    public int? DurationSeconds { get; set; }

    // Navigation properties (optional if using EF Core)
    public Game? Game { get; set; }
}
