using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tamagotchi_pet.Models
{
    public class UpdatePetDTO
    {
        public float xp { get; set; }
        public float food { get; set; }
        public float stamina { get; set; }
        public float water { get; set; }
    }
}