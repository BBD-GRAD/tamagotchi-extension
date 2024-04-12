using System.ComponentModel.DataAnnotations;

namespace tamagotchi_pet.DTOs
{
    public class ThemeDTO
    {
        [Required]
        public int ThemeId { get; set; }
    }
}