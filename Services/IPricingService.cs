using System;
using GameStore.Api.Models;

namespace GameStore.Api.Services;

public interface IPricingService
{
    Task<GamePrice> SetPriceForGameAsync(Guid gameId, long pricePaise, string currency = "INR", DateTimeOffset? effectiveFrom = null);
    Task<GamePrice?> GetActivePriceAsync(Guid gameId, string currency = "INR");
    Task<List<GamePrice>> GetPriceHistoryAsync(Guid gameId, string currency = "INR");
}
