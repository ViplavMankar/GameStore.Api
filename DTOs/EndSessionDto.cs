using System;

namespace GameStore.Api.DTOs;

public class EndSessionDto
{
    // The session ID returned when starting the session
    public Guid SessionId { get; set; }

    // Optional: additional info
    // public int? DurationSeconds { get; set; }
}