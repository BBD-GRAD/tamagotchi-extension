using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs
{
    public class CreatePetDTO
    {
        [Required]
        public string PetName { get; set; }
    }
}