using System;

namespace GameStore.Api.DTOs;

public class SetGamePriceRequest
{
    public long PricePaise { get; set; }
    public string Currency { get; set; } = "INR";
    public DateTimeOffset? EffectiveFrom { get; set; }
}