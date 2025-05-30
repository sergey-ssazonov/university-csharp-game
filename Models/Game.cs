using System;
using System.Collections.Generic;

namespace HardwareKiller.Models
{
    public class Game
    {
        public int CellSize   { get; } = 32;
        public int GridWidth  { get; } = 20;
        public int GridHeight { get; } = 15;

        public Player            Player    { get; }
        public List<Cat>         Cats      { get; }
        public List<Item>        Items     { get; }
        public List<Hazard>      Hazards   { get; }
        public List<Hazard>      Obstacles { get; }

        public Game()
        {
            var rng = new Random();

            // 1) Игрок и коты
            Player = new Player(0, 0);
            Cats = new List<Cat>
            {
                new Cat(GridWidth - 1, GridHeight - 1),
                new Cat(0, GridHeight - 1)
            };

            // 2) Коллекции
            Items     = new List<Item>();
            Hazards   = new List<Hazard>();
            Obstacles = new List<Hazard>();

            // 3) Занятые клетки
            var occupied = new HashSet<(int x, int y)> { (Player.X, Player.Y) };
            foreach (var c in Cats) occupied.Add((c.X, c.Y));

            // 4) Обожжённые пиксели (стены)
            for (int i = 0; i < 30; i++)
            {
                int x, y;
                do { x = rng.Next(GridWidth); y = rng.Next(GridHeight); }
                while (!occupied.Add((x, y)));
                Obstacles.Add(new Hazard(x, y, HazardType.BurnedPixel));
            }

            // 5) Файлы и папки (Items)
            for (int i = 0; i < 10; i++)
            {
                int x, y;
                do { x = rng.Next(GridWidth); y = rng.Next(GridHeight); }
                while (!occupied.Add((x, y)));
                var type = (i % 2 == 0) ? ItemType.File : ItemType.Folder;
                Items.Add(new Item(x, y, type));
            }

            // 6) Опасные предметы (Hazards)
            var bads = new[] {
                HazardType.Shootdown,
                HazardType.TrashBox,
                HazardType.BlueWindows
            };
            foreach (var t in bads)
            {
                int x, y;
                do { x = rng.Next(GridWidth); y = rng.Next(GridHeight); }
                while (!occupied.Add((x, y)));
                Hazards.Add(new Hazard(x, y, t));
            }
        }

        public void MovePlayer(int dx, int dy)
        {
            int nx = Player.X + dx, ny = Player.Y + dy;
            // границы и стены
            if (nx < 0 || nx >= GridWidth || ny < 0 || ny >= GridHeight) return;
            if (Obstacles.Exists(o => o.X == nx && o.Y == ny)) return;
            Player.MoveTo(nx, ny);
        }
    }
}
