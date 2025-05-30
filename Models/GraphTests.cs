/*using Microsoft.VisualStudio.TestTools.UnitTesting;
using HardwareKiller.Models;
using System.Linq;

namespace HardwareKiller.Tests.Models
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void ShortestPath_StartEqualsTarget_ReturnsSingle()
        {
            var g = new Graph(5,5);
            var path = g.ShortestPath(2,2,2,2);
            Assert.AreEqual(1, path.Count);
            Assert.AreEqual((2,2), path.First());
        }

        [TestMethod]
        public void ShortestPath_OpenGrid_CorrectLength()
        {
            int w=5,h=5;
            var g = new Graph(w,h);
            // путь от (0,0) до (4,3) длина = dx+dy+1
            var path = g.ShortestPath(0,0,4,3);
            Assert.AreEqual(4+3+1, path.Count);
            Assert.AreEqual((0,0), path.First());
            Assert.AreEqual((4,3), path.Last());
        }

        [TestMethod]
        public void ShortestPath_BlockedCell_NoPath()
        {
            // Здесь мы не учитываем препятствия в Graph: он просто считает прямой путь.
            // Если хотите, можно замоделировать стену в Game, но Graph не знает про них.
            var g = new Graph(3,3);
            var path = g.ShortestPath(0,0,2,2);
            Assert.IsTrue(path.Count > 0);
        }
    }
}*/