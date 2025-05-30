namespace HardwareKiller.Models
{
    public abstract class Entity
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        protected Entity(int x, int y)
        {
            X = x;
            Y = y;
        }
        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}