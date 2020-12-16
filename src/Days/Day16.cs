using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 16)]
    public class Day16 : BaseDay
    {
        public override string PartOne(string input)
        {
            var (rules, _, otherTickets) = input.LinesPreservingEmpty().ParseLines();
            return SieveTickets(rules, otherTickets).invalidFieldValues.Sum().ToString();
        }

        public override string PartTwo(string input)
        {
            var (rules, myTicket, otherTickets) = input.LinesPreservingEmpty().ParseLines();
            var validTickets = SieveTickets(rules, otherTickets).validTickets;
            var classifications = ClassifyPositions(rules, myTicket, validTickets);
            return classifications.Where(t => t.fieldName.StartsWith("departure"))
                .Aggregate(1L, (l, tuple) => l * myTicket[tuple.fieldPosition]).ToString();
        }

        private static (IEnumerable<int> invalidFieldValues, List<int[]> validTickets) SieveTickets(
            Dictionary<string, (int[] firstRange, int[] secondRange)> rules, IEnumerable<int[]> otherTickets)
        {
            var invalidFieldValues = new List<int>();
            var validTickets = new List<int[]>();
            foreach (var ticket in otherTickets)
            {
                var validTicket = true;
                foreach (var field in ticket)
                {
                    var validField = false;
                    foreach (var (_, (firstRange, secondRange)) in rules)
                    {
                        validField = validField || (firstRange.Contains(field) || secondRange.Contains(field));
                    }
                    if (!validField) invalidFieldValues.Add(field);
                    validTicket = validTicket && validField;
                }
                if (validTicket) validTickets.Add(ticket);
            }
            return (invalidFieldValues, validTickets);
        }

        private static IEnumerable<(string fieldName, int fieldPosition)> ClassifyPositions(
            Dictionary<string, (int[] firstRange, int[] secondRange)> rules, IReadOnlyCollection<int> myTicket,
            IReadOnlyCollection<int[]> validTickets)
        {
            var fieldClassifications = new List<string>[myTicket.Count];
            for (var i = 0; i < myTicket.Count; i++)
                fieldClassifications[i] = new List<string>(rules.Select(v => v.Key));
            for (var i = 0; i < myTicket.Count; i++)
            {
                foreach (var (key, (firstRange, secondRange)) in rules)
                {
                    foreach (var ticket in validTickets)
                    {
                        if (firstRange.Contains(ticket[i]) || secondRange.Contains(ticket[i])) continue;
                        fieldClassifications[i].Remove(key);
                        break;
                    }
                }
            }
            var classified = new List<(string name, int position)>();
            while (fieldClassifications.Any(x => x.Count > 1))
            {
                var nextReduceValue = fieldClassifications.Where((x, index) =>
                    x.Count == 1 && classified.All(c => c.position != index)).Single().First();
                for (var i = 0; i < fieldClassifications.Length; i++)
                {
                    if (fieldClassifications[i].Count == 1)
                    {
                        if (fieldClassifications[i].First() == nextReduceValue) classified.Add((nextReduceValue, i));
                    }
                    else fieldClassifications[i].Remove(nextReduceValue);
                }
            }
            classified.Add(fieldClassifications.Where(x => classified.All(c => c.name != x.First()))
                .Select(t => (t.First(), fieldClassifications.IndexOf(t))).Single());
            return classified;
        }
    }

    internal static class ArrayExtensions
    {
        public static IEnumerable<string> LinesPreservingEmpty(this string input)
        {
            return input.Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.TrimEntries);
        }


        internal static (Dictionary<string, (int[] firstRange, int[] secondRange)> rules, int[] numbersOnMyTicket,
            List<int[]> numbersOnOtherTickets) ParseLines(this IEnumerable<string> arg)
        {
            var data = arg.ToArray();
            var index = 0;
            var rules = new Dictionary<string, (int[] firstRange, int[] secondRange)>();
            while (data[index] != string.Empty)
            {
                var parts = data[index].Split(new[] {": ", "-", " or "}, StringSplitOptions.RemoveEmptyEntries);

                rules.Add(parts[0],
                    (Enumerable.Range(int.Parse(parts[1]), int.Parse(parts[2]) - int.Parse(parts[1]) + 1).ToArray(),
                        Enumerable.Range(int.Parse(parts[3]), int.Parse(parts[4]) - int.Parse(parts[3]) + 1)
                            .ToArray()));
                index++;
            }
            index += 2;
            var numbersOnMyTicket = data[index].Integers().ToArray();
            index += 3;
            var numbersOnOtherTickets = new List<int[]>();
            while (index < data.Length)
            {
                numbersOnOtherTickets.Add(data[index++].Integers().ToArray());
            }
            return (rules, numbersOnMyTicket, numbersOnOtherTickets);
        }
    }
}