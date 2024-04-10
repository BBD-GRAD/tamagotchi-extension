using System.ComponentModel.DataAnnotations;

namespace tamagotchi_pet.DTOs
{
    public class XPDTO
    {
        [Required]
        public long xp { get; set; }
    }
}