using System;
using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IProfileService
{
    Task<PlayerProfileDto> GetPlayerProfileAsync(Guid userId);
}
