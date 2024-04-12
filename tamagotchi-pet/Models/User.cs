using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace tamagotchi_pet.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string UserId { get; set; }

        [ForeignKey("Theme")]
        public int ThemeId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public long Highscore { get; set; }
    }
}