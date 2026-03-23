using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IAchievementService
{
    Task<List<AchievementUnlockedDto>> EvaluateAchievementsAsync(Guid userId);
}
