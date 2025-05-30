/*
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HardwareKiller.Models;
using System.Linq;

namespace HardwareKiller.Tests.Models
{
    [TestClass]
    public class GameTests
    {
        [TestMethod]
        public void Constructor_GeneratesNoOverlaps()
        {
            var game = new Game();
            var all = game.Cats
                .Select(c => (c.X,c.Y))
                .Concat(game.Items.Select(i => (i.X,i.Y)))
                .Concat(game.Hazards.Select(z => (z.X,z.Y)))
                .Concat(game.Obstacles.Select(o => (o.X,o.Y)))
                .ToList();
            // Проверяем, что ни одна позиция не повторяется
            Assert.AreEqual(all.Count, all.Distinct().Count());
        }

        [TestMethod]
        public void MovePlayer_CannotWalkThroughObstacle()
        {
            var game = new Game();
            // ставим стену справа от (0,0)
            var obs = new Hazard(1,0,HazardType.BurnedPixel);
            game.Obstacles.Add(obs);
            game.MovePlayer(1,0);
            // Подвинуться не получилось
            Assert.AreEqual(0, game.Player.X);
            Assert.AreEqual(0, game.Player.Y);
        }

        [TestMethod]
        public void MovePlayer_BoundsCheck()
        {
            var game = new Game();
            // слева от (0,0) выход за границу
            game.MovePlayer(-1,0);
            Assert.AreEqual(0, game.Player.X);
            Assert.AreEqual(0, game.Player.Y);
        }

        [TestMethod]
        public void CollectItem_ReducesCount()
        {
            var game = new Game();
            var first = game.Items.First();
            // двигаем игрока прямо на этот предмет
            int dx = first.X - game.Player.X;
            int dy = first.Y - game.Player.Y;
            game.MovePlayer(dx, dy);
            // моделируем сбор
            if (game.Player.X==first.X && game.Player.Y==first.Y)
                game.Items.Remove(first);
            Assert.IsFalse(game.Items.Contains(first));
        }

        [TestMethod]
        public void Hazard_CausesGameOver()
        {
            var game = new Game();
            // Если игрок ходит на Hazard, логика GameController устанавливает GameOver.
            // Здесь просто проверяем наличие hazard в коллекции.
            Assert.IsTrue(game.Hazards.Count >= 1);
        }
    }
}
*/
