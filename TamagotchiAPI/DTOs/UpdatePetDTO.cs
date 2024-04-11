using System.ComponentModel.DataAnnotations;

namespace TamagotchiAPI.DTOs
{
    public class UpdatePetDTO
    {
        public long? XP { get; set; }

        public double? Stamina { get; set; }
        
        public double? Health { get; set; }
        
        public double? Food { get; set; }
        
        public double? Water { get; set; }
    }
}