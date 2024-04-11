using System.ComponentModel.DataAnnotations;

namespace tamagotchi_pet.DTOs
{
    public class ThemeDTO
    {
        [Required]
        public int themeid { get; set; }
    }
}