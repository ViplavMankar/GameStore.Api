using System;

namespace GameStore.Api.DTOs;

public class StartSessionDto
{
    // The ID of the game being played
    public Guid GameId { get; set; }

    // Optional: any metadata like device, browser, etc.
    public string? DeviceInfo { get; set; }
}