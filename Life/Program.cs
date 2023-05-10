using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;

namespace cli_life
{
    public class Configuration
    {
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }
        public double density { get; set; }
    }

    public class Cell
    {
        public bool IsAlive;
        public readonly List<Cell> neighbors = new List<Cell>();
        private bool IsAliveNext;
        public void DetermineNextLiveState()
        {
            int liveNeighbors = neighbors.Where(x => x.IsAlive).Count();
            if (IsAlive)
                IsAliveNext = liveNeighbors == 2 || liveNeighbors == 3;
            else
                IsAliveNext = liveNeighbors == 3;
        }
        public void Advance()
        {
            IsAlive = IsAliveNext;
        }
    }
    public class Board
    {
        public readonly Cell[,] Cells;
        public readonly int CellSize;

        public int Columns { get { return Cells.GetLength(0); } }
        public int Rows { get { return Cells.GetLength(1); } }
        public int Width { get { return Columns * CellSize; } }
        public int Height { get { return Rows * CellSize; } }

        public Board(int width, int height, int cellSize, double liveDensity)
        {
            CellSize = cellSize;

            Cells = new Cell[width / cellSize, height / cellSize];
            for (int x = 0; x < Columns; x++)
                for (int y = 0; y < Rows; y++)
                    Cells[x, y] = new Cell();

            ConnectNeighbors();
        }

        public void FileConfiguration(string file)
        {
            string[] data = File.ReadAllLines(file);
            char[][] config = new char[Rows][];
            for (int i = 0; i < data.Length; ++i)
            {
                config[i] = new char[Columns];
                for (int j = 0; j < Rows; ++j)
                {
                    config[i][j] = data[i][j];
                }
            }
            for (int i = 0; i < Rows; ++i)
            {
                for (int j = 0; j < Columns; ++j)
                {
                    if (config[i][j] == '*')
                    {
                        Cells[i, j].IsAlive = true;
                    }
                }
            }
        }

        readonly Random rand = new Random();
        public void Randomize(string file)
        {
            string json = File.ReadAllText(file);
            Configuration data = JsonSerializer.Deserialize<Configuration>(json);
            double density = data.density;
            foreach (var cell in Cells)
                cell.IsAlive = rand.NextDouble() < density;
        }

        public void Advance()
        {
            foreach (var cell in Cells)
                cell.DetermineNextLiveState();
            foreach (var cell in Cells)
                cell.Advance();
        }
        private void ConnectNeighbors()
        {
            for (int x = 0; x < Columns; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    int xL = (x > 0) ? x - 1 : Columns - 1;
                    int xR = (x < Columns - 1) ? x + 1 : 0;

                    int yT = (y > 0) ? y - 1 : Rows - 1;
                    int yB = (y < Rows - 1) ? y + 1 : 0;

                    Cells[x, y].neighbors.Add(Cells[xL, yT]);
                    Cells[x, y].neighbors.Add(Cells[x, yT]);
                    Cells[x, y].neighbors.Add(Cells[xR, yT]);
                    Cells[x, y].neighbors.Add(Cells[xL, y]);
                    Cells[x, y].neighbors.Add(Cells[xR, y]);
                    Cells[x, y].neighbors.Add(Cells[xL, yB]);
                    Cells[x, y].neighbors.Add(Cells[x, yB]);
                    Cells[x, y].neighbors.Add(Cells[xR, yB]);
                }
            }
        }
        public int BlocksAmount()
        {
            int amount = 0;
            for (int i = 1; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 2; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j, i + 1].IsAlive && Cells[j + 1, i].IsAlive && Cells[j + 1, i + 1].IsAlive)
                    {
                        if (!Cells[j - 1, i - 1].IsAlive && !Cells[j, i - 1].IsAlive && !Cells[j + 1, i - 1].IsAlive && !Cells[j + 2, i - 1].IsAlive
                        && !Cells[j - 1, i + 2].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive
                        && !Cells[j - 1, i].IsAlive && !Cells[j + 2, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 2, i + 2].IsAlive)
                        {
                            amount++;
                        }
                    }
                }
            }
            return amount;
        }
        public int BoxesAmount()
        {
            int amount = 0;
            for (int i = 0; i < Rows - 2; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j - 1, i].IsAlive && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 2].IsAlive && !Cells[j + 1, i + 2].IsAlive)
                    {
                        amount++;
                    }
                }
            }
            return amount;
        }
        public int HivesAmount()
        {
            int amount = 0;
            for (int i = 0; i < Rows - 3; i++)
            {
                for (int j = 1; j < Columns - 1; j++)
                {
                    if (Cells[j, i].IsAlive && Cells[j - 1, i + 1].IsAlive && Cells[j - 1, i + 2].IsAlive
                    && Cells[j, i + 3].IsAlive && Cells[j + 1, i + 1].IsAlive && Cells[j + 1, i + 2].IsAlive
                    && !Cells[j, i + 1].IsAlive && !Cells[j, i + 2].IsAlive && !Cells[j - 1, i].IsAlive
                    && !Cells[j + 1, i].IsAlive && !Cells[j - 1, i + 3].IsAlive && !Cells[j + 1, i + 3].IsAlive)
                    {
                        amount++;
                    }
                }
            }
            return amount;
        }
        public int SymmetriesAmount()
        {
            return BoxesAmount() + HivesAmount() + BlocksAmount();
        }
    }
    public class Game
    {
        static Board board;
        public int Reset(string file, string read_from)
        {
            string json = File.ReadAllText(read_from);
            Configuration data = JsonSerializer.Deserialize<Configuration>(json);
            int width = data.width;
            int height = data.height;
            int cellSize = data.cellSize;
            double density = data.density;
            board = new Board(width, height, cellSize, density);
            board.FileConfiguration(file);
            return board.Width * board.Height;
        }
        public int Render()
        {
            int amount = 0;
            for (int row = 0; row < board.Rows; ++row)
            {
                for (int col = 0; col < board.Columns; ++col)
                {
                    var cell = board.Cells[col, row];
                    if (cell.IsAlive)
                    {
                        Console.Write('*');
                        amount++;
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
            }
            return amount;
        }
        public void ToFile()
        {
            char[][] config = new char[20][];
            for (int k = 0; k < 20; k++)
            {
                config[k] = new char[50];
            }
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Columns; j++)
                {
                    var cell = board.Cells[j, i];
                    if (cell.IsAlive)
                    {
                        config[i][j] = '*';
                    }
                    else
                    {
                        config[i][j] = ' ';
                    }
                }
            }
            File.Create("result.txt").Close();
            using (StreamWriter writer = new StreamWriter("result.txt", true))
            {
                for (int i = 0; i < config.Length; i++)
                {
                    string str = new string(config[i]);
                    writer.WriteLine(str);
                }
            }
        }

        public (int allCells, int aliveCells, int Iters) Run(string file, string read_from)
        {
            int[] list = { -1, -1, -1, -1, -1 };
            int iters = 0;
            int alive_cells = 0;
            int all_cells = 0;

            all_cells = Reset(file, read_from);

            while (true)
            {
                iters++;
                try
                {
                    Console.Clear();
                }
                catch
                {
                }
                alive_cells = Render();
                list[iters % 5] = alive_cells;
                if ((list[0] == list[1]) && (list[0] == list[2]) && (list[0] == list[3]) && (list[0] == list[4]))
                {
                    break;
                }
                board.Advance();
                Thread.Sleep(150);
            }

            Console.WriteLine("\n\tБлоков на доске: " + board.BlocksAmount());
            Console.WriteLine("\tЯщиков на доске: " + board.BoxesAmount());
            Console.WriteLine("\tУльев на доске: " + board.HivesAmount());

            (int, int, int) cells = (all_cells, alive_cells, iters - 2);
            return cells;
        }
    }

    class Programm
    {
        static void Main(string[] args)
        {
            Game life = new Game();
            var cells = life.Run("start.txt", "settings.json");
            var alive = cells.aliveCells;
            var dead = cells.allCells - cells.aliveCells;
            var dead_dens = ((double)cells.aliveCells / cells.allCells);
            var alive_dens = ((double)(cells.allCells - cells.aliveCells) / cells.allCells);

            Console.Write("\n\tЖивых клеток на доске: " + alive);
            Console.Write("\n\tМертвых клеток на доске: " + dead);
            Console.Write("\n\tПлотность живых клеток на доске: " + dead_dens);
            Console.Write("\n\tПлотность живых клеток на доске: " + alive_dens);

            life.ToFile();

            Console.Write("\n\n\tСтабильность на " + (cells.Iters) + " итерации.\n\n");
        }
    }
}
