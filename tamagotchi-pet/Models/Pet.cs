using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tamagotchi_pet.Models
{
    [Table("Pets")]
    public class Pet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int PetId { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public string PetName { get; set; }

        [Required]
        public long XP { get; set; }

        [Required]
        public double Health { get; set; }

        [Required]
        public double Food { get; set; }

        [Required]
        public double Water { get; set; }

        [Required]
        public double Stamina { get; set; }

        [Required]
        public virtual User User { get; set; }  // Lazy loading
    }
}