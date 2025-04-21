using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IGameService
{
    Task<IEnumerable<GameReadDto>> GetAllAsync();
    Task<GameReadDto> CreateAsync(GameCreateDto dto, Guid authorId);
    Task<bool> AddToCollectionAsync(Guid userId, Guid gameId);
}
