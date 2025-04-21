using GameStore.Api.Data;
using GameStore.Api.DTOs;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class GameService : IGameService
{
    private readonly GameStoreDbContext _context;

    public GameService(GameStoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GameReadDto>> GetAllAsync()
    {
        return await _context.Games
            .Select(g => new GameReadDto
            {
                Id = g.Id,
                Title = g.Title,
                Description = g.Description,
                GameUrl = g.GameUrl,
                ThumbnailUrl = g.ThumbnailUrl,
                AuthorUserId = g.AuthorUserId,
                CreatedAt = g.CreatedAt
            }).ToListAsync();
    }

    public async Task<GameReadDto> CreateAsync(GameCreateDto dto, Guid authorId)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ThumbnailUrl = dto.ThumbnailUrl,
            GameUrl = dto.GameUrl,
            AuthorUserId = authorId,
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return new GameReadDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            GameUrl = game.GameUrl,
            ThumbnailUrl = game.ThumbnailUrl,
            AuthorUserId = game.AuthorUserId,
            CreatedAt = game.CreatedAt
        };
    }

    public async Task<bool> AddToCollectionAsync(Guid userId, Guid gameId)
    {
        var exists = await _context.UserCollections
            .AnyAsync(c => c.UserId == userId && c.GameId == gameId);

        if (exists) return false;

        _context.UserCollections.Add(new UserCollection
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GameId = gameId
        });

        await _context.SaveChangesAsync();
        return true;
    }
}
