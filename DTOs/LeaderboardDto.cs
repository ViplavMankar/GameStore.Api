using System;

namespace GameStore.Api.DTOs;

public class LeaderboardDto
{
    public int Rank { get; set; }
    public string Username { get; set; }
    public int Value { get; set; }
}