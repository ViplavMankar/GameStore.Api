using System;
using Microsoft.AspNetCore.Mvc;
using GameStore.Api.Services;
using GameStore.Api.DTOs;

namespace GameStore.Api.Controllers;

[ApiController]
[Route("api/games/{gameId:guid}/price")]
public class GamePricingController : ControllerBase
{
    private readonly IPricingService _pricing;

    public GamePricingController(IPricingService pricing)
    {
        _pricing = pricing;
    }

    /// GET /api/games/{gameId}/price?currency=INR
    [HttpGet]
    public async Task<ActionResult<GetPriceResult>> GetPrice([FromRoute] Guid gameId, [FromQuery] string currency = "INR")
    {
        currency = (currency ?? "INR").ToUpperInvariant();
        var active = await _pricing.GetActivePriceAsync(gameId, currency);

        if (active is null)
            return Ok(new GetPriceResult(gameId, currency, false, 0));

        return Ok(new GetPriceResult(gameId, active.Currency, active.PricePaise > 0, active.PricePaise));
    }

    /// PUT /api/games/{gameId}/price
    /// Body: { "pricePaise": 49900, "currency": "INR" }
    [HttpPut]
    // [Authorize(Roles="Admin")] // uncomment when you wire auth
    public async Task<ActionResult<GamePriceResponse>> SetPrice([FromRoute] Guid gameId, [FromBody] SetGamePriceRequest req)
    {
        var price = await _pricing.SetPriceForGameAsync(
            gameId,
            req.PricePaise,
            string.IsNullOrWhiteSpace(req.Currency) ? "INR" : req.Currency.ToUpperInvariant(),
            req.EffectiveFrom
        );

        var resp = new GamePriceResponse(
            price.GameId,
            price.PricePaise,
            price.Currency,
            price.IsActive,
            price.EffectiveFrom,
            price.EffectiveTo
        );

        return Ok(resp);
    }

    /// GET /api/games/{gameId}/price/history?currency=INR
    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<GamePriceResponse>>> GetHistory([FromRoute] Guid gameId, [FromQuery] string currency = "INR")
    {
        currency = (currency ?? "INR").ToUpperInvariant();
        var list = await _pricing.GetPriceHistoryAsync(gameId, currency);
        var resp = list.Select(p => new GamePriceResponse(p.GameId, p.PricePaise, p.Currency, p.IsActive, p.EffectiveFrom, p.EffectiveTo));
        return Ok(resp);
    }
}
