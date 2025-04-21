namespace GameStore.Api.Models;

public class UserCollection
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid GameId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public Game Game { get; set; }
}
