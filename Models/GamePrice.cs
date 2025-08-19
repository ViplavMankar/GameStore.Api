using System;

namespace GameStore.Api.Models;

public class GamePrice
{
    public int Id { get; set; }

    // FK -> Game
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;

    // Money (store in paise)
    public long PricePaise { get; set; }
    public string Currency { get; set; } = "INR";  // ISO 4217, e.g. INR, USD

    // State & validity
    public bool IsActive { get; set; } = true;
    public DateTimeOffset EffectiveFrom { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EffectiveTo { get; set; }
    public DateTimeOffset CreatedUtc { get; set; } = DateTimeOffset.UtcNow;
}
