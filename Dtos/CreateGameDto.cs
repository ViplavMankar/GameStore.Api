using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGameDto(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(1, 10000)] decimal Price,
    DateOnly ReleaseDate
);
