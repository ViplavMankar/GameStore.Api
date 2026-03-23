using System;
using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;

namespace GameStore.Api.Services;

public class LeaderboardBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LeaderboardBackgroundService> _logger;
    private readonly HttpClient _authClient;

    public LeaderboardBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<LeaderboardBackgroundService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _authClient = httpClientFactory.CreateClient("AuthService");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Leaderboard Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateLeaderboard();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leaderboard");
            }

            // Run every 5 minutes
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task UpdateLeaderboard()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();

        var now = DateTime.UtcNow;

        var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
        var weekEnd = weekStart.AddDays(7);

        var data = await db.GameSessions
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                TotalPlayTime = g.Sum(x => x.DurationSeconds ?? 0)
            })
            .OrderByDescending(x => x.TotalPlayTime)
            .ToListAsync();

        // // Clear old leaderboard
        var oldEntries = db.LeaderboardEntries;

        db.LeaderboardEntries.RemoveRange(oldEntries);

        // Insert new ranked data
        int rank = 1;

        foreach (var item in data)
        {
            db.LeaderboardEntries.Add(new LeaderboardEntry
            {
                Id = Guid.NewGuid(),
                UserId = item.UserId,
                Username = await ResolveUsernameAsync(item.UserId) ?? "Unknown",
                TotalPlayTimeSeconds = item.TotalPlayTime,
                Rank = rank++,
                LastUpdated = now
            });
        }

        await db.SaveChangesAsync();

        _logger.LogInformation("Leaderboard updated successfully");
    }
    private async Task<string?> ResolveUsernameAsync(Guid? userId)
    {
        if (userId == null)
        {
            return "Anonymous";
        }
        try
        {
            var response = await _authClient.GetAsync($"api/users/{userId.ToString()}");
            if (!response.IsSuccessStatusCode)
                return "Unknown";

            var user = await response.Content.ReadFromJsonAsync<UserReadDto>();
            return user?.Username ?? "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
    private class UserReadDto
    {
        public string Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }
}