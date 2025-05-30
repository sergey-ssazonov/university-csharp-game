namespace HardwareKiller.Models
{
    public class Cat : Entity
    {
        // предыдущие координаты, чтобы понимать направление движения
        public int PrevX { get; private set; }
        public int PrevY { get; private set; }

        public Cat(int x, int y) : base(x, y)
        {
            PrevX = x;
            PrevY = y;
        }

        // при перемещении сохраняем старую позицию
        public void MoveTo(int x, int y)
        {
            PrevX = X;
            PrevY = Y;
            base.MoveTo(x, y);
        }
    }
}