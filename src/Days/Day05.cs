using System;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 5)]
    public class Day05 : BaseDay
    {
        // lol, 7 characters for 128 possibilities doesn't that remind
        // us of something? :) And again 3 chars for 8 possibilities.
        public override string PartOne(string input)
        {
            var passes = input.Lines();
            return passes.Max(SeatId).ToString();
        }

        // Linq FTW
        public override string PartTwo(string input)
        {
            var orderedPasses = input.ParseStrings(SeatId).OrderBy(x => x).ToArray();
            return Enumerable.Range(orderedPasses[0], orderedPasses[^1] - orderedPasses[0] + 1)
                .Except(orderedPasses).First().ToString();
        }

        private static int SeatId(string arg)
        {
            // swap letters to binary digits and return result
            var binaryString = arg.Replace('B', '1').Replace('F', '0').Replace('R', '1').Replace('L', '0');
            return Convert.ToInt32(binaryString, 2);
        }
    }
}