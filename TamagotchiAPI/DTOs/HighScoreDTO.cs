using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs;

public class HighScoreDTO
{
    [Required]
    public long Highscore { get; set; }
}