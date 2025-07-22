using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Api.Models
{
    public class GameRating
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Game Game { get; set; }
    }
}