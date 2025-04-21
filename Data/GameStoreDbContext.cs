using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;

namespace GameStore.Api.Data;

public class GameStoreDbContext : DbContext
{
    public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<UserCollection> UserCollections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserCollection>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        modelBuilder.Entity<UserCollection>()
            .HasOne(x => x.Game)
            .WithMany()
            .HasForeignKey(x => x.GameId);
    }
}
