using System.ComponentModel.DataAnnotations;

namespace tamagotchi_pet.DTOs
{
    public class HighScoreDTO
    {
        [Required]
        public long Highscore { get; set; }
    }
}