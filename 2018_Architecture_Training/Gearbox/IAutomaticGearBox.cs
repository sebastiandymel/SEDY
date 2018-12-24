namespace Gearbox
{
    public interface IAutomaticGearBox
    {
        int Gear { get; set; }
        int GetMaxGears();
        void Drive();
        void Park();
        void Neutral();
        void Reverse();
    }

    public interface IGearCalculator
    {
        int CalculateGear(int rpm, int currentGear);
    }

    public class EcoModeCalculator : IGearCalculator
    {
        public int CalculateGear(int rpm, int currentGear)
        {
            if (rpm > 2500 && currentGear < 5)
            {
                return currentGear + 1;
            }
            return currentGear;
        }
    }

    public class SportModeCalculator : IGearCalculator
    {
        public int CalculateGear(int rpm, int currentGear)
        {
            if (rpm > 5000 && currentGear < 5)
            {
                return currentGear + 1;
            }
            return currentGear;
        }
    }
}