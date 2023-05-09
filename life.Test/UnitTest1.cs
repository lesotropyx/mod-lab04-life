using Microsoft.VisualStudio.TestTools.UnitTesting;
using cli_life;

namespace StringLibraryTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Game g = new Game();
            var cells = g.Run("start.txt", "settings.json");
            Assert.AreEqual(cells.Iters, 71);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Game g = new Game();
            var cells = g.Run("start.txt", "settings.json");
            Assert.AreEqual(cells.aliveCells, 26);
        }

        [TestMethod]
        public void TestMethod3()
        {
            Game g = new Game();
            var cells = g.Run("start.txt", "settings.json");
            Assert.AreEqual(cells.allCells, 1000);
        }

        [TestMethod]
        public void TestMethod4()
        {
            Game g = new Game();
            var cells = g.Run("start.txt", "settings.json");
            Assert.AreEqual(((double)(cells.allCells - cells.aliveCells) / cells.allCells), 0,974);
        }

        [TestMethod]
        public void TestMethod5()
        {
            Board board = new Board(50, 20, 1, 0.5);
            board.FileConfiguration("box.txt");
            Assert.AreEqual(board.BoxesAmount(), 2);
        }
    }
}