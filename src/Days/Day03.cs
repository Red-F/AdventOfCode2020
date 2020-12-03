using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 3)]
    public class Day03 : BaseDay
    {
        public override string PartOne(string input)
        {
            // read input into grid, and traverse it
            var map = input.CreateCharGrid();
            return TreeCount(map, (3, 1)).ToString();
        }

        public override string PartTwo(string input)
        {
            // read input into grid
            var map = input.CreateCharGrid();
            // traverse grid for all slopes, multiplying the results
            var result =
                new[] {(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)}.Aggregate(1L,
                    (current, slope) => current * TreeCount(map, slope));
            return result.ToString();
        }

        private static int TreeCount(char[,] map, (int right, int down) valueTuple)
        {
            // walk the map, simulating infinite right by wrapping x 
            var (right, down) = valueTuple;
            var lowerBoundary = map.GetUpperBound(1);
            var rightBoundary = map.GetUpperBound(0);
            var x = 0;
            var y = 0;
            var trees = 0;

            while (y < lowerBoundary)
            {
                y += down;
                x = (x + right) % (rightBoundary + 1);
                if (map[x, y] == '#') trees++;
            }

            return trees;
        }
    }
}