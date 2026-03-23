using System;
using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class DailyChallengeGeneratorService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<LeaderboardBackgroundService> _logger;

    public DailyChallengeGeneratorService(IServiceScopeFactory scopeFactory,
        ILogger<LeaderboardBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Daily Challenge Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await DelayUntilNextRun(stoppingToken);
            await GenerateDailyChallenges();
        }
    }
    private async Task DelayUntilNextRun(CancellationToken token)
    {
        // var now = DateTime.UtcNow;

        // var nextRun = now.Date.AddSeconds(20);

        // if (nextRun <= now)
        // {
        //     nextRun = nextRun.AddDays(1);
        // }

        // var delay = nextRun - now;

        // await Task.Delay(delay, token);
        await Task.Delay(TimeSpan.FromMinutes(15), token);
    }
    private async Task GenerateDailyChallenges()
    {
        using var scope = _scopeFactory.CreateScope();
        var _context = scope.ServiceProvider.GetRequiredService<GameStoreDbContext>();

        var today = DateTime.UtcNow.Date;

        var exists = await _context.DailyChallenges
            .AnyAsync(x => x.Date == today);

        if (exists)
        {
            _logger.LogInformation("Daily Challenges already exist for today, skipping generation");
            return;
        }

        var random = new Random();

        var gameTarget = random.Next(3, 8); // 3–7 games
        var timeTarget = random.Next(10, 31); // 10–30 minutes

        var challenges = new List<DailyChallenge>
        {
            new DailyChallenge
            {
                Id = Guid.NewGuid(),
                Title = "Play Games",
                Description = $"Play {gameTarget} games",
                TargetValue = gameTarget,
                XPReward = gameTarget * 10,
                Date = today
            },
            new DailyChallenge
            {
                Id = Guid.NewGuid(),
                Title = "Play Time",
                Description = $"Play {timeTarget} minutes",
                TargetValue = timeTarget * 60, // store in seconds
                XPReward = timeTarget * 2,
                Date = today
            }
        };

        _context.DailyChallenges.AddRange(challenges);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Daily Challenges updated successfully");
    }
}
