using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using tamagotchi_pet.Models;

namespace tamagotchi_pet.Models
{
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PetId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public string PetName { get; set; }

        [Required]
        public long XP { get; set; }

        [Required]
        public double Water { get; set; }

        [Required]
        public double Food { get; set; }

        [Required]
        public double Stamina { get; set; }

        [Required]
        public double Health { get; set; }

        [Required]
        public virtual User User { get; set; }  // Lazy loading
    }
}