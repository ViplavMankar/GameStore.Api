using System;

namespace GameStore.Api.DTOs;

public record GetPriceResult(
    Guid GameId,
    string Currency,
    bool IsPaid,
    long PricePaise // 0 if free / none
);