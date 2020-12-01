using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 1)]
    public class Day01 : BaseDay
    {
        public override string PartOne(string input)
        {
            // Create an ascending array from the input
            var items = input.Integers().OrderBy(x => x).ToArray();
            // get all possible pairs of two entries from input, sum them and stop when 2020 is found
            var rc = items.GetCombinations(2).First(pair => pair.Sum() == 2020);
            // this pair multiplied is the answer
            return (rc[0] * rc[1]).ToString();
        }

        public override string PartTwo(string input)
        {
            // Create an ascending array from the input
            var items = input.Integers().OrderBy(x => x).ToArray();
            // get all possible pairs of three entries from input, sum them and stop when 2020 is found
            var rc = items.GetCombinations(3).First(pair => pair.Sum() == 2020);
            // this pair multiplied is the answer
            return (rc[0] * rc[1] * rc[2]).ToString();
        }
    }
}
