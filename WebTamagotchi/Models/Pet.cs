using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebTamagotchi.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string UserId { get; set; }
        public string PetName { get; set; }
        public long XP { get; set; }
        public double Health { get; set; }
        public double Food { get; set; }
        public double Water { get; set; }
        public double Stamina { get; set; }
        public virtual User User { get; set; }  // Lazy loading
    }
}
