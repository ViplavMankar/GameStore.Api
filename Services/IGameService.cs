using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IGameService
{
    Task<IEnumerable<GameReadDto>> GetAllAsync();
    Task<GameReadDto> CreateAsync(GameCreateDto dto, Guid authorId);
    Task<GameReadDto> EditAsync(GameEditDto dto, Guid authorId);
    Task<bool> AddToCollectionAsync(Guid userId, Guid gameId);
    Task<bool> DeleteAsync(Guid id, Guid authorId);
    Task<bool> RemoveFromCollectionAsync(Guid id, Guid userId);
}
