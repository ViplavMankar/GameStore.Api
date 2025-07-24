using System;
using GameStore.Api.Data;
using GameStore.Api.DTOs;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Services;

public class CollectionService : ICollectionService
{
    private readonly GameStoreDbContext _context;

    public CollectionService(GameStoreDbContext context)
    {
        _context = context;
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

    public async Task<bool> RemoveFromCollectionAsync(Guid id, Guid userId)
    {
        var collectionEntry = await _context.UserCollections.FirstOrDefaultAsync(c => c.UserId == userId && c.GameId == id);

        if (collectionEntry == null) return false;

        _context.UserCollections.Remove(collectionEntry);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<GameReadDto>> GetUserCollectionAsync(Guid userId)
    {
        return await (from c in _context.UserCollections
                      join g in _context.Games on c.GameId equals g.Id
                      where c.UserId == userId
                      select new GameReadDto
                      {
                          Id = g.Id,
                          Title = g.Title,
                          ThumbnailUrl = g.ThumbnailUrl,
                          Description = g.Description,
                          GameUrl = g.GameUrl
                      }).ToListAsync();
    }

    public async Task<bool> IsGameSavedAsync(Guid userId, Guid gameId)
    {
        return await _context.UserCollections.AnyAsync(c => c.UserId == userId && c.GameId == gameId);
    }
}
