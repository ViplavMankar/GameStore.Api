using System;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;
using GameStore.Api.Data;

namespace GameStore.Api.Services;

public class PricingService : IPricingService
{
    private readonly GameStoreDbContext _db;
    public PricingService(GameStoreDbContext db) => _db = db;

    public async Task<GamePrice> SetPriceForGameAsync(Guid gameId, long pricePaise, string currency = "INR", DateTimeOffset? effectiveFrom = null)
    {
        // Deactivate current active price(s) for this game/currency
        var actives = await _db.GamePrices
            .Where(p => p.GameId == gameId && p.Currency == currency && p.IsActive)
            .ToListAsync();

        foreach (var p in actives)
        {
            p.IsActive = false;
            p.EffectiveTo = DateTimeOffset.UtcNow;
        }

        var newPrice = new GamePrice
        {
            GameId = gameId,
            PricePaise = pricePaise,
            Currency = currency,
            IsActive = true,
            EffectiveFrom = effectiveFrom ?? DateTimeOffset.UtcNow
        };

        _db.GamePrices.Add(newPrice);
        await _db.SaveChangesAsync();
        return newPrice;
    }

    public async Task<GamePrice?> GetActivePriceAsync(Guid gameId, string currency = "INR")
    {
        return await _db.GamePrices
            .Where(p => p.GameId == gameId && p.Currency == currency && p.IsActive)
            .OrderByDescending(p => p.EffectiveFrom)
            .FirstOrDefaultAsync();
    }

    public async Task<List<GamePrice>> GetPriceHistoryAsync(Guid gameId, string currency = "INR")
    {
        return await _db.GamePrices
            .Where(p => p.GameId == gameId && p.Currency == currency)
            .OrderByDescending(p => p.EffectiveFrom)
            .ToListAsync();
    }
}
