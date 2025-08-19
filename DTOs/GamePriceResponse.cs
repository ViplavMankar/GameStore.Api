using System;

namespace GameStore.Api.DTOs;

public record GamePriceResponse(
    Guid GameId,
    long PricePaise,
    string Currency,
    bool IsActive,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveTo
);