using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 7)]
    public class Day07 : BaseDay
    {
        public override string PartOne(string input)
        {
            var rules = input.ParseStrings(RuleParser).ToArray();
            return BagsContainingBags("shiny gold", rules).Count().ToString();
        }

        public override string PartTwo(string input)
        {
            var rules = input.ParseStrings(RuleParser).ToArray();
            // don't forget to subtract one for the shiny gold bag itself :)
            return (CountBags("shiny gold", rules) - 1).ToString();
        }

        private static IEnumerable<string> BagsContainingBags(string colorOfThisBag,
            (string color, Dictionary<string, int> map)[] rules)
        {
            var rc = rules.Where(t => t.map.ContainsKey(colorOfThisBag)).Select(t => t.color).Distinct().ToList();
            return !rc.Any()
                ? System.Array.Empty<string>()
                : rc.SelectMany(c => BagsContainingBags(c, rules)).Concat(rc).Distinct();
        }

        private static int CountBags(string colorOfThisBag,
            (string color, Dictionary<string, int> map)[] rules)
        {
            return 1 + rules.Single(t => t.color == colorOfThisBag).map.Sum(kv => kv.Value * CountBags(kv.Key, rules));
        }

        // parse a single color and its rules 
        private static (string color, Dictionary<string, int> map) RuleParser(string arg)
        {
            var words = arg.Words().ToArray();
            var containsMap = new Dictionary<string, int>();
            for (var i = 4; i < words.Length; i += 4)
            {
                if (words[i] == "no") continue;
                containsMap.Add($"{words[i + 1]} {words[i + 2]}", int.Parse(words[i]));
            }
            return ($"{words[0]} {words[1]}", containsMap);
        }
    }
}