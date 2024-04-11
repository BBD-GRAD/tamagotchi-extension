using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BlazorTamagotchi.Models
{
    public class User
    {
        public string UserId { get; set; }

        public int ThemeId { get; set; }

        public string Email { get; set; }

        public long Highscore { get; set; }
    }
}
