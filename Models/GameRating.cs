using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameStore.Api.Models
{
    public class GameRating
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid GameId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Game Game { get; set; }
    }
}