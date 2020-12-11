using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 11)]
    public class Day11 : BaseDay
    {
        public override string PartOne(string input) =>
            StabilisationCount(input.CreateCharGrid(), MovePeople).ToString();

        public override string PartTwo(string input) =>
            StabilisationCount(input.CreateCharGrid(), ExtendedMovePeople).ToString();

        private static int StabilisationCount(char[,] grid, Func<int, int, char[,], char> transformer)
        {
            var previousGrid = new char[grid.GetLength(0), grid.GetLength(1)];
            do
            {
                Array.Copy(grid, 0, previousGrid, 0, grid.Length);
                previousGrid.Transform(grid, transformer);
            } while (grid.Count('#') != previousGrid.Count('#') && grid.Count('L') != previousGrid.Count('L'));

            return grid.Count('#');
        }

        private static char MovePeople(int x, int y, char[,] grid) =>
            grid[x, y] == '.'
                ? '.'
                : (grid[x, y], grid.Neighbours(x, y).Count(t => t == '#')) switch
                {
                    ('L', 0) => '#',
                    ('#', var count) when count >= 4 => 'L',
                    _ => grid[x, y]
                };

        private static char ExtendedMovePeople(int x, int y, char[,] grid)
        {
            var rc = grid[x, y] == '.'
                ? '.'
                : (grid[x, y], grid.ExtendedNeighbours(x, y).Count(c => c == '#')) switch
                {
                    ('L', 0) => '#',
                    ('#', var count) when count >= 5 => 'L',
                    _ => grid[x, y]
                };
            return rc;
        }
    }

    public static class DayArrayExtensions
    {
        public static int Count(this char[,] grid, char c) => grid.Cast<char>().Count(x => x == c);

        public static void Transform(this char[,] previousGrid, char[,] newGrid,
            Func<int, int, char[,], char> transformer)
        {
            for (var y = 0; y < previousGrid.GetLength(1); y++)
            for (var x = 0; x < previousGrid.GetLength(0); x++)
                newGrid[x, y] = transformer(x, y, previousGrid);
        }

        private static readonly (int, int)[] Directions =
            {(0, 1), (0, -1), (1, 0), (-1, 0), (1, 1), (-1, -1), (-1, 1), (1, -1)};

        public static IEnumerable<char> Neighbours(this char[,] grid, int x, int y)
        {
            foreach (var (dx, dy) in Directions)
            {
                var currentX = x + dx;
                var currentY = y + dy;
                if (currentX < grid.GetLength(0) && currentX >= 0 && currentY < grid.GetLength(1) && currentY >= 0)
                    yield return grid[currentX, currentY];
            }
        }

        public static IEnumerable<char> ExtendedNeighbours(this char[,] grid, int x, int y)
        {
            foreach (var (dx, dy) in Directions)
            {
                var currentX = x + dx;
                var currentY = y + dy;
                while (currentX < grid.GetLength(0) && currentX >= 0 && currentY < grid.GetLength(1) && currentY >= 0)
                {
                    if (grid[currentX, currentY] != '.')
                    {
                        yield return grid[currentX, currentY];
                        break;
                    }

                    currentX += dx;
                    currentY += dy;
                }
            }
        }
    }
}