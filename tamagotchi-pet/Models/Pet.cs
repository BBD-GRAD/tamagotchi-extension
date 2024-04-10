using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TamagotchiAPI.Models
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
        public int XP { get; set; }

        [Required]
        public int Happiness { get; set; }

        [Required]
        public virtual User User { get; set; }  // Lazy loading
    }
}