using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface ITrendingGameService
{
    Task<List<TrendingGameDto>> GetTrendingGamesAsync();
    Task<List<GamePlaytimeDto>> GetTotalPlaytimePerGameAsync();
    Task<List<DailyActivePlayersDto>> GetDailyActivePlayersAsync(int days);
}
