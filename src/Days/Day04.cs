using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 4)]
    public class Day04 : BaseDay
    {
        public override string PartOne(string input)
        {
            // split into individual passports and validate
            var passports = input.LineGroups(Environment.NewLine + Environment.NewLine);
            return passports.Count(ValidatePassport1).ToString();
        }

        public override string PartTwo(string input)
        {
            // split into individual passports and validate
            var passports = input.LineGroups(Environment.NewLine + Environment.NewLine).Where(ValidatePassport1);
            return passports.Count(ValidatePassport2).ToString();
        }

        private static bool ValidatePassport1(string arg)
        {
            // split into fields and make sure there are 8 fields or otherwise 7 with no cid
            var fields = arg.Split(new[] {" ", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            return fields.Length == 8 || (fields.Length == 7 && !fields.Any(s => s.Contains("cid")));
        }

        private static bool ValidatePassport2(string arg)
        {
            // split into fields and validate
            var fields = arg.Split(new[] {" ", Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split(":", StringSplitOptions.RemoveEmptyEntries));
            return fields.All(field => FieldValidators[field[0]](field[1]));
        }

        private static readonly Dictionary<string, Func<string, bool>> FieldValidators =
            new Dictionary<string, Func<string, bool>>()
            {
                {
                    "byr",
                    x => x.Length == 4 && int.Parse(x).InRange(1920, 2002)
                },
                {"iyr", x => x.Length == 4 && int.Parse(x).InRange(2010, 2020)},
                {"eyr", x => x.Length == 4 && int.Parse(x).InRange(2020, 2030)},
                {
                    "hgt",
                    x => x.Length > 2 && x.Substring(x.Length - 2) switch
                    {
                        "in" => int.Parse(x.Substring(0, x.Length - 2)).InRange(59, 76),
                        "cm" => int.Parse(x.Substring(0, x.Length - 2)).InRange(150, 193),
                        _ => false
                    }
                },
                {
                    "hcl",
                    x => x.Length == 7 && x[0] == '#' && x.Substring(1).All(c =>
                        c.InRange('0', '9') || c.InRange('a', 'f'))
                },
                {"ecl", x => x.Length == 3 && new[] {"amb", "blu", "brn", "gry", "grn", "hzl", "oth"}.Contains(x)},
                {"pid", x => x.Length == 9 && x.All(c => c.InRange('0', '9'))},
                {"cid", _ => true},
            };
    }
}