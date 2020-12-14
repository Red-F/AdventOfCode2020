using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    [Day(2020, 14)]
    public class Day14 : BaseDay
    {
        private static readonly Regex Rgx = new(@"mem\[([0-9]+)\][^0-9]+([0-9]+)");
        private const ulong Bits36 = 0xFFFFFFFFF;
        private const ulong Bit36 = 0x1000000000;

        public override string PartOne(string input)
        {
            var program = input.Lines().ToArray();
            return ExecuteOne(program).ToString();
        }

        public override string PartTwo(string input)
        {
            var program = input.Lines().ToArray();
            return ExecuteTwo(program).ToString();
        }

        private static ulong ExecuteOne(IEnumerable<string> program)
        {
            var memory = new Dictionary<ulong, ulong>();
            (ulong andMask, ulong orMask) masks = (Bits36, 0);
            foreach (var s in program)
            {
                if (s[1] == 'a') masks = MaskParserOne(s);
                else
                {
                    var (address, value) = InstructionParser(s);
                    memory[address] = (value & masks.andMask) | masks.orMask;
                }
            }

            return memory.Aggregate(0UL, (current, kv) => current + kv.Value);
        }

        private static ulong ExecuteTwo(IEnumerable<string> program)
        {
            var memory = new Dictionary<ulong, ulong>();
            (ulong andMask, ulong orMask, ulong floatMask) masks = (Bits36, 0, 0);
            foreach (var s in program)
            {
                if (s[1] == 'a') masks = MaskParserTwo(s);
                else
                {
                    var (instructionAddress, value) = InstructionParser(s);
                    var addresses = UnFloat(new List<ulong> {(instructionAddress & masks.andMask) | masks.orMask},
                        masks.floatMask, 1);
                    foreach (var address in addresses) memory[address] = value;
                }
            }

            return memory.Aggregate(0UL, (current, kv) => current + kv.Value);
        }

        private static (ulong andMask, ulong orMask) MaskParserOne(string arg)
        {
            var mask = arg.Split(' ')[2];
            (ulong andMask, ulong orMask) masks = (0, 0);
            ulong bit = 1;
            for (var i = 35; i >= 0; i--, bit *= 2)
                masks = mask[i] switch
                {
                    'X' => (masks.andMask + bit, masks.orMask),
                    '1' => (masks.andMask + bit, masks.orMask + bit),
                    _ => masks
                };
            return masks;
        }

        private static (ulong andMask, ulong orMask, ulong floatMask) MaskParserTwo(string arg)
        {
            var mask = arg.Split(' ')[2];
            (ulong andMask, ulong orMask, ulong floatMask) masks = (0, 0, 0);
            ulong bit = 1;
            for (var i = 35; i >= 0; i--, bit *= 2)
                masks = mask[i] switch
                {
                    'X' => (masks.andMask, masks.orMask, masks.floatMask + bit),
                    '1' => (masks.andMask, masks.orMask + bit, masks.floatMask),
                    '0' => (masks.andMask + bit, masks.orMask, masks.floatMask),
                    _ => masks
                };
            return masks;
        }

        private static IEnumerable<ulong> UnFloat(List<ulong> addresses, ulong floatMask, ulong bit)
        {
            while (true)
            {
                if (bit >= Bit36) return addresses;
                if ((floatMask & bit) != 0)
                {
                    var rc = new List<ulong>();
                    foreach (var address in addresses)
                    {
                        rc.Add(address & ~bit);
                        rc.Add(address | bit);
                    }

                    addresses = rc;
                }
                bit *= 2;
            }
        }

        private static (ulong address, ulong value) InstructionParser(string arg)
        {
            var match = Rgx.Match(arg);
            return (ulong.Parse(match.Groups[1].Value) & Bits36, ulong.Parse(match.Groups[2].Value) & Bits36);
        }
    }
}