using System;

namespace GameStore.Api.DTOs;

public class CreateAchievementDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ConditionType { get; set; } = string.Empty;
    public int TargetValue { get; set; }
}
