using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Garage.Codess.Samples
{
    using Cell = Tuple<int, int>;
    using Generation = HashSet<Tuple<int, int>>;

    public class Program
    {
        // Constrains the width of the playing field
        private static int GridWidth
        {
            get { return Math.Min(Console.WindowWidth, 30); }
        }

        // Constrains the height of the playing field
        private static int GridHeight
        {
            get { return Math.Min(Console.WindowHeight, 30); }
        }

        // Create a seed generation. For now, just a simple blinker
        // somewhere around the center of the grid.
        private static Generation BuildSeedGeneration()
        {
            int x = GridWidth / 2;
            int y = GridHeight / 2;

            return new Generation()
            {
                new Cell(x, y - 1),
                new Cell(x, y),
                new Cell(x, y + 1)
            };
        }

        private static void Render(Generation g)
        {
            try
            {
                // Clear the screen first
                Console.Clear();
                Console.CursorVisible = false;

                // Draw each live node in the generation
                foreach (Cell c in g)
                {
                    Console.SetCursorPosition(c.Item1, c.Item2);
                    Console.Write('*');
                }
            }
            catch (IOException)
            {
                // Most likely we're running with output redirection. Simply swallow
                // these exceptions and don't render anything in that case.
            }
        }

        private static bool IsAlive(Generation g, int x, int y)
        {
            // The generation is a sparse matrix, so mere existence implies life.
            // Man, that's deep.
            return g.Contains(new Cell(x, y));
        }

        private static int CountLiveNeighbors(Generation g, int x, int y)
        {
            int runningTotal = 0;
            int[] directions = new int[] { -1, 0, 1 };

            foreach (int xOff in directions)
            {
                foreach (int yOff in directions)
                {
                    // Avoid the edge cases:
                    // - No cell is a neighbor of itself
                    if (xOff == 0 && yOff == 0)
                    {
                        continue;
                    }

                    // - Literal edges. Don't walk off the ends of the matrix. The game of life as originally
                    // specified has an infinite grid, but we're mere mortals.
                    int newX = x + xOff;
                    int newY = y + yOff;

                    if (newX < 0 || newX > GridWidth || newY < 0 || newY > GridHeight)
                    {
                        continue;
                    }

                    // If this adjacent cell is alive, then note its contribution to the
                    // total number of live neighbors.
                    if (IsAlive(g, newX, newY))
                    {
                        ++runningTotal;
                    }
                }
            }

            return runningTotal;
        }

        private static Generation Evolve(Generation g)
        {
            Generation next = new Generation();

            // Populate the next generation from the current generation. The rules
            // that govern how the next generation is populated are:
            //
            // 1. A live cell with fewer than two live neighbors dies (underpopulation)
            // 2. A live cell with two or three live neighbors lives on (thriving)
            // 3. A live cell with more than three live neighbors dies (overpopulation)
            // 4. A dead cell with exactly three live neighbors comes to life (reproduction)
            //
            // This is super brute force.
            for (int x = 0; x < GridWidth; ++x)
            {
                for (int y = 0; y < GridHeight; ++y)
                {
                    // How many neighbors does that location have?
                    int neighborCount = CountLiveNeighbors(g, x, y);

                    // Determine if this cell is alive in the next generation
                    if (neighborCount == 3 || (neighborCount == 2 && IsAlive(g, x, y)))
                    {
                        next.Add(new Cell(x, y));
                    }
                }
            }

            return next;
        }

        public static void Main(string[] args)
        {
            // Play the game of life. Get the initial generation, and draw each successive
            // generation that evolves from it.
            Generation currentGeneration = BuildSeedGeneration();
            for (;;)
            {
                Render(currentGeneration);
                currentGeneration = Evolve(currentGeneration);
            }
        }
    }
}
