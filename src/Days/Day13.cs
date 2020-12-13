using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 13)]
    public class Day13 : BaseDay
    {
        public override string PartOne(string input)
        {
            var data = input.Lines().ToArray();
            var earliestTimeLeaving = int.Parse(data[0]);
            var busIds = data[1].Words().Where(x => x != "x").Select(int.Parse).ToArray();
            var timeLeaving = earliestTimeLeaving - 1;
            int id;
            do
            {
                timeLeaving++;
                id = busIds.FirstOrDefault(busId => timeLeaving % busId == 0);
            } while (id == 0);
            return ((timeLeaving - earliestTimeLeaving) * id).ToString();
        }

        public override string PartTwo(string input)
        {
            var busIds = input.Lines().Last().Words().Select(x => x == "x" ? 0 : long.Parse(x)).ToArray();
            (long id, long delta)[] idsAndDeltas = busIds.Where(id => id != 0)
                .Select(id => (id, (long) (busIds.Length - 1 - busIds.IndexOf(id)))).ToArray();
            return (Sieve(idsAndDeltas) - idsAndDeltas.First().delta).ToString();
        }

        // See https://en.wikipedia.org/wiki/Chinese_remainder_theorem
        private static long Sieve(IEnumerable<(long id, long delta)> tuples)
        {
            (long id, long delta)[] data = tuples.Select(t => (t.id, t.delta % t.id)).OrderByDescending(t => t.Item2)
                .ToArray();
            var endTimestamp = data[0].delta;
            var increment = data[0].id;
            for (var i = 0; i < data.Length - 1; i++)
            {
                while (true)
                {
                    var mod = endTimestamp % data[i + 1].id;
                    if (mod == data[i + 1].delta) break;
                    endTimestamp += increment;
                }
                increment *= data[i + 1].id;
            }
            return endTimestamp;
        }
    }
}