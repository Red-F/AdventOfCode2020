using System;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 9)]
    public class Day09 : BaseDay
    {
        private const int PreambleSize = 25;

        public override string PartOne(string input)
        {
            var numbers = input.Longs().ToArray();
            return FindInvalidElement(numbers).ToString();
        }

        public override string PartTwo(string input)
        {
            var numbers = input.Longs().ToArray();
            return FindEncryptionWeakness(numbers).ToString();
        }

        private static long FindInvalidElement(long[] numbers)
        {
            var preamble = numbers.Take(PreambleSize).ToArray();
            var dataStream = numbers.Skip(PreambleSize);
            foreach (var element in dataStream)
            {
                if (InvalidElement(preamble, element)) return element;
            }
            throw new InvalidOperationException("Should not be reached");
        }

        private static bool InvalidElement(long[] preamble, long element)
        {
            for (var i = 0; i < preamble.Length - 1; i++)
            for (var j = i + 1; j < preamble.Length; j++)
            {
                if (element != preamble[i] + preamble[j]) continue;
                Array.Copy(preamble, 1, preamble, 0, preamble.Length - 1);
                preamble[^1] = element;
                return false;
            }
            return true;
        }

        private static long FindEncryptionWeakness(long[] numbers)
        {
            var invalidElement = FindInvalidElement(numbers);
            for (var i = 0; numbers[i] < invalidElement; i++)
            {
                var sum = numbers[i];
                for (var j = i + 1; sum < invalidElement; j++)
                {
                    sum += numbers[j];
                    if (sum != invalidElement) continue;
                    var slice = numbers[i..j];
                    return (slice.Min() + slice.Max());
                }
            }
            throw new InvalidOperationException("Should not be reached");
        }
    }
}