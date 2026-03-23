using System;

namespace GameStore.Api.Models;

public class UserDailyChallengeProgress
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid ChallengeId { get; set; }

    public int CurrentProgress { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }
}