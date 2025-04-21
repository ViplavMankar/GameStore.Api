namespace GameStore.Api.DTOs;

public class GameReadDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public string GameUrl { get; set; }
    public Guid AuthorUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}
