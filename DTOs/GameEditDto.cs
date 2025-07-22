namespace GameStore.Api.DTOs;

public class GameEditDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ThumbnailUrl { get; set; }
    public string GameUrl { get; set; }
}
