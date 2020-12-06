using System;
using System.Linq;

namespace AdventOfCode.Days
{
    /// <summary>
    /// Who said C# was more readable than F#? ;-)
    /// Heavy Linq but still performant
    /// </summary>
    [Day(2020, 6)]
    public class Day06 : BaseDay
    {
        public override string PartOne(string input) =>
            input.LineGroups(Environment.NewLine + Environment.NewLine).Sum(CountGroup1).ToString();

        public override string PartTwo(string input)
        {
            return input.LineGroups(Environment.NewLine + Environment.NewLine)
                .Sum(s => CountGroup2(s.Where(char.IsLetter).ToArray(), s.Lines().Count())).ToString();
        }


        private static int CountGroup1(string arg) => arg.Where(char.IsLetter).Distinct().Count();

        private static int CountGroup2(char[] arg, int count) =>
            arg.Distinct().Count(c => arg.Count(x => x == c) == count);
    }
}