using GameStore.Api.DTOs;

namespace GameStore.Api.Services;

public interface IGameService
{
    Task<IEnumerable<GameReadDto>> GetAllAsync();
    Task<GameReadDto> CreateAsync(GameCreateDto dto, Guid authorId);
    Task<GameReadDto> EditAsync(Guid id, GameEditDto dto, Guid authorId);
    Task<bool> DeleteAsync(Guid id, Guid authorId);

    Task<IEnumerable<GameReadDto>> GetMyGamesAsync(Guid userId);
    Task<GameReadDto?> GetMySingleGameAsync(Guid userId, Guid gameId);
}
