using System;

namespace GameStore.Api.Models;

public class DailyChallenge
{
    public Guid Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public int TargetValue { get; set; }

    public int XPReward { get; set; } // 💥 NEW

    public DateTime Date { get; set; }
}
