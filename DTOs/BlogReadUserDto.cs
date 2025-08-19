using System;

namespace GameStore.Api.DTOs;

public class BlogReadUserDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid? AuthorUserId { get; set; }
    public string? AuthorUsername { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
