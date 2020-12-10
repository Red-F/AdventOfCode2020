using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 10)]
    public class Day10 : BaseDay
    {
        private static readonly IEnumerable<int> ValidRange = Enumerable.Range(1, 3);

        public override string PartOne(string input)
        {
            var adapterRatings = string.Concat(input, Environment.NewLine, "0").Integers().OrderBy(x => x).ToArray();
            return JoltageDistribution(adapterRatings).ToString();
        }

        public override string PartTwo(string input)
        {
            var adapterRatings = string.Concat(input, Environment.NewLine, "0").Integers().OrderBy(x => x).ToArray();
            var rc = AdapterArrangements(adapterRatings, adapterRatings[^1] + 3, new Dictionary<int, long> {{0, 1L}});
            return rc.ToString();
        }

        private static int JoltageDistribution(int[] adapterRatings)
        {
            var gaps = adapterRatings.Select(x =>
            {
                var pos = adapterRatings.IndexOf(x);
                return pos < adapterRatings.Length - 1 ? adapterRatings[pos + 1] - adapterRatings[pos] : 3;
            }).ToArray();
            return gaps.Count(x => x == 1) * gaps.Count(x => x == 3);
        }

        private static long AdapterArrangements(int[] adapterRatings, int currentJoltage,
            IDictionary<int, long> knownCounts)
        {
            if (knownCounts.ContainsKey(currentJoltage)) return knownCounts[currentJoltage];
            var suitableAdapters =
                adapterRatings.Where(x => x < currentJoltage && ValidRange.Contains(currentJoltage - x));
            knownCounts[currentJoltage] = suitableAdapters.Sum(x => AdapterArrangements(adapterRatings, x, knownCounts));
            return knownCounts[currentJoltage];
        }
    }
}