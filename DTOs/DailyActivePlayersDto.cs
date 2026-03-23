using System;

namespace GameStore.Api.DTOs;

public class DailyActivePlayersDto
{
    public DateTime Date { get; set; }

    public int ActivePlayers { get; set; }
}