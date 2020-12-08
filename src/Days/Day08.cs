using System;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 8)]
    public class Day08 : BaseDay
    {
        public override string PartOne(string input)
        {
            var program = input.ParseStrings(ParseInstruction).ToArray();
            return ExecuteProgram(program).accumulator.ToString();
        }

        public override string PartTwo(string input)
        {
            var program = input.ParseStrings(ParseInstruction).ToArray();
            for (var pc = 0; pc < program.Length; pc++)
            {
                if (program[pc].opcode == "acc") continue;
                program[pc].opcode = program[pc].opcode == "nop" ? "jmp" : "nop";
                var (status, accumulator) = ExecuteProgram(program);
                if (status) return accumulator.ToString();
                program[pc].opcode = program[pc].opcode == "nop" ? "jmp" : "nop";
                for (var i = 0; i < program.Length; i++) program[i].count = 0;
            }

            throw new InvalidOperationException("should never come here");
        }

        private static (string opcode, int operand, int count) ParseInstruction(string arg)
        {
            var words = arg.Words().ToArray();
            return (words[0], int.Parse(words[1]), 0);
        }

        // Run the program return tuple indicating completed and accumulator value
        private static (bool status, int accumulator) ExecuteProgram((string opcode, int operand, int count)[] program)
        {
            var accumulator = 0;
            var pc = 0;
            while (pc < program.Length && program[pc].count == 0)
            {
                program[pc].count++;
                var _ = program[pc].opcode switch
                {
                    "acc" => accumulator += program[pc].operand,
                    "jmp" => pc += program[pc].operand - 1,
                    _ => 0
                };
                pc++;
            }

            return (pc >= program.Length, accumulator);
        }
    }
}