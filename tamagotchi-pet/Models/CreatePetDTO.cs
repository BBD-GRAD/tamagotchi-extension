using System.ComponentModel.DataAnnotations;

namespace tamagotchi_pet.DTOs
{
    public class CreatePetDTO
    {
        [Required]
        public string PetName { get; set; }
    }
}