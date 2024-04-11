namespace BlazorTamagotchi.Models
{
    public class Pet
    {
        public int PetId { get; set; }
        public string UserId { get; set; }
        public string PetName { get; set; }
        public int XP { get; set; }
        public int Happiness { get; set; }
        public virtual User User { get; set; }  // Lazy loading
    }
}
