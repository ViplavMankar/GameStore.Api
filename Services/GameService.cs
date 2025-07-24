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
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt
        };
    }

    public async Task<GameReadDto> EditAsync(GameEditDto dto, Guid authorId)
    {
        var game = await _context.Games.FindAsync(dto.Id);
        if (game == null)
        {
            throw new KeyNotFoundException("Game Not Found");
        }
        if (game.AuthorUserId != authorId)
        {
            throw new UnauthorizedAccessException("You are not allowed to edit this game!");
        }
        game.Title = dto.Title ?? game.Title;
        game.ThumbnailUrl = dto.ThumbnailUrl ?? game.ThumbnailUrl;
        game.Description = dto.Description ?? game.Description;
        game.GameUrl = dto.GameUrl ?? game.GameUrl;

        game.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return new GameReadDto
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            ThumbnailUrl = game.ThumbnailUrl,
            GameUrl = game.GameUrl,
            AuthorUserId = game.AuthorUserId,
            CreatedAt = game.CreatedAt,
            UpdatedAt = game.UpdatedAt,
        };
    }

    public async Task<bool> DeleteAsync(Guid id, Guid authorId)
    {
        var game = await _context.Games.FindAsync(id);
        if (game == null)
        {
            return false;
        }
        if (game.AuthorUserId != authorId)
        {
            throw new UnauthorizedAccessException("You are not allowed to edit this game!");
        }
        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        return true;
    }
}
