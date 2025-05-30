namespace HardwareKiller.Models
{
    public enum HazardType { Shootdown, TrashBox, BlueWindows, BurnedPixel }
    public class Hazard : Entity
    {
        public HazardType Type { get; }
        public Hazard(int x, int y, HazardType type) : base(x, y)
            => Type = type;
    }
}