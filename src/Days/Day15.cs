using System;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 15)]
    public class Day15 : BaseDay
    {
        public override string PartOne(string input)
        {
            var startingNumbers = input.Longs().ToArray();
            var memory = new long[2020];
            Array.Copy(startingNumbers, memory, startingNumbers.Length);
            for (var i = startingNumbers.Length; i < 2020; i++)
            for (var j = i - 2; j >= 0; j--)
            {
                if (memory[j] != memory[i - 1]) continue;
                memory[i] = i - j - 1;
                break;
            }
            return memory[2019].ToString();
        }

        public override string PartTwo(string input)
        {
            var startingNumbers = input.Longs().ToArray();
            var memory = new long[30000000];
            for (var i = 0; i < startingNumbers.Length - 1; i++) memory[startingNumbers[i]] = i + 1;
            var nextNumber = startingNumbers[^1];
            for (var i = startingNumbers.Length + 1; i <= 30000000; i++)
            {
                if (memory[nextNumber] == 0)
                {
                    memory[nextNumber] = i - 1;
                    nextNumber = 0;
                }
                else
                {
                    var newNumber = i - 1 - memory[nextNumber];
                    memory[nextNumber] = i - 1;
                    nextNumber = newNumber;
                }
            }
            return nextNumber.ToString();
        }
    }
}