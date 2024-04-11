namespace WebTamagotchi.Models
{
    public class ViewModel
    {
        public System.Timers.Timer timer;
        public Pet pet;
        public PetStates currentState;
        public int gracePeriod;
        public const float healthDrainConst = 100 / 60;
    }

    public enum PetStates
    {
        Hungry,
        Thirsty,
        Sleepy,
        Happy,
        Eating,
        Drinking,
        Resting
    }
}
