using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs
{
    public class UpdatePetDTO
    {
        public int? XP { get; set; }

        public int? Happiness { get; set; }
    }
}