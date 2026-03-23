using System;
using GameStore.Api.DTOs;
using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class TrendingGameService : ITrendingGameService
{
    private readonly GameStoreDbContext _context;

    public TrendingGameService(GameStoreDbContext context)
    {
        _context = context;
    }
    public async Task<List<TrendingGameDto>> GetTrendingGamesAsync()
    {
        var last24Hours = DateTime.UtcNow.AddHours(-24);

        var trendingGames = await _context.GameSessions
            .Where(s => s.StartedAt >= last24Hours)
            .GroupBy(s => s.GameId)
            .Select(g => new
            {
                GameId = g.Key,
                SessionCount = g.Count()
            })
            .OrderByDescending(x => x.SessionCount)
            .Take(10)
            .ToListAsync();

        var gameIds = trendingGames.Select(t => t.GameId).ToList();

        var games = await _context.Games
            .Where(g => gameIds.Contains(g.Id))
            .ToDictionaryAsync(g => g.Id);

        return trendingGames.Select(t => new TrendingGameDto
        {
            GameId = t.GameId,
            Title = games[t.GameId].Title,
            SessionCount = t.SessionCount
        }).ToList();
    }
    public async Task<List<GamePlaytimeDto>> GetTotalPlaytimePerGameAsync()
    {
        var sessions = await _context.GameSessions
            .Where(s => s.EndedAt != null)
            .Include(s => s.Game)
            .ToListAsync();

        var result = sessions
            .GroupBy(s => new { s.GameId, s.Game.Title })
            .Select(g => new GamePlaytimeDto
            {
                GameId = g.Key.GameId,
                Title = g.Key.Title,
                TotalMinutes = g.Sum(s =>
                    (s.EndedAt!.Value - s.StartedAt).TotalMinutes)
            })
            .OrderByDescending(x => x.TotalMinutes)
            .ToList();

        return result;
    }
    public async Task<List<DailyActivePlayersDto>> GetDailyActivePlayersAsync(int days)
    {
        var startDate = DateTime.UtcNow.Date.AddDays(-days);

        var result = await _context.GameSessions
            .Where(s => s.StartedAt >= startDate)
            .GroupBy(s => s.StartedAt.Date)
            .Select(g => new DailyActivePlayersDto
            {
                Date = g.Key,
                ActivePlayers = g.Select(x => x.UserId).Distinct().Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return result;
    }
}
