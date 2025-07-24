using System;

namespace GameStore.Api.DTOs;

public class CollectionCreateDto
{
    public Guid GameId { get; set; }
    public Guid UserId { get; set; }
}
