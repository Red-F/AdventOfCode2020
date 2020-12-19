using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OneOf;

namespace AdventOfCode.Days
{
    [Day(2020, 19)]
    public class Day19 : BaseDay
    {
        public override string PartOne(string input)
        {
            var data = input.LineGroups(Environment.NewLine + Environment.NewLine).ToArray();
            return MessagesMatchingRule0(data, BuildMatchesOne).ToString();
        }

        public override string PartTwo(string input)
        {
            var data = input.LineGroups(Environment.NewLine + Environment.NewLine).ToArray();
            return MessagesMatchingRule0(data, BuildMatchesTwo).ToString();
        }

        private static long MessagesMatchingRule0(string[] data,
            Action<Dictionary<int, OneOf<AndNode, OrNode, Leaf>>> builder)
        {
            var rules = BuildRules(data[0].Lines().ToArray());
            builder(rules);
            return CountMatches(data[1].Lines(), "^" + rules[0].AsT2.Value + "$");
        }

        private static int CountMatches(IEnumerable<string> lines, string regex)
        {
            var r = new Regex(regex);
            return lines.Count(l => r.IsMatch(l));
        }

        private static void BuildMatchesOne(Dictionary<int, OneOf<AndNode, OrNode, Leaf>> rules)
        {
            while (!rules.All(r => IsTrueLeaf(r.Value)))
                foreach (var rule in rules)
                    rules[rule.Key] = ReduceNode(rule.Value, rules);
        }

        private static void BuildMatchesTwo(Dictionary<int, OneOf<AndNode, OrNode, Leaf>> rules)
        {
            while (rules.Count(r => !IsTrueLeaf(r.Value)) > 3)
                foreach (var rule in rules)
                    rules[rule.Key] = ReduceNode(rule.Value, rules);
            var value31 = rules[31].AsT2.Value;
            var value42 = rules[42].AsT2.Value;
            // You may have other rules in your input to replace
            rules[8] = new Leaf($"({value42})+");
            rules[11] = new Leaf($"(?<Count_A>{value42})+(?<-Count_A>{value31})+(?(Count_A)(?!))");
            rules[0] = ReduceNode(rules[0], rules);
            rules[0] = ReduceNode(rules[0], rules);
        }
        
        private static OneOf<AndNode, OrNode, Leaf> ReduceNode(OneOf<AndNode, OrNode, Leaf> node,
            Dictionary<int, OneOf<AndNode, OrNode, Leaf>> rules)
        {
            if (IsLeaf(node) && !IsTrueLeaf(node))
            {
                var subrule = rules[int.Parse(node.AsT2.Value)];
                if (IsTrueLeaf(subrule)) return subrule;
            }
            else if (IsAnd(node))
            {
                var andRule = node.AsT0;
                if (andRule.Sequence.All(IsTrueLeaf))
                {
                    return new Leaf(andRule.Sequence.Aggregate(string.Empty,
                        (current, x) => current + x.AsT2.Value));
                }

                for (var j = 0; j < andRule.Sequence.Length; j++)
                    andRule.Sequence[j] = ReduceNode(andRule.Sequence[j], rules);
                return andRule;
            }
            else if (IsOr(node))
            {
                var orRule = node.AsT1;
                if (orRule.Sequence.All(IsTrueLeaf))
                {
                    var left = orRule.Sequence[0].AsT2.Value;
                    var right = orRule.Sequence[1].AsT2.Value;
                    return new Leaf('(' + left + '|' + right + ')');
                }
                for (var j = 0; j < orRule.Sequence.Length; j++)
                    orRule.Sequence[j] = ReduceNode(orRule.Sequence[j], rules);
                return orRule;
            }
            return node;
        }

        private static bool IsAnd(OneOf<AndNode, OrNode, Leaf> node) => node.IsT0;
        private static bool IsOr(OneOf<AndNode, OrNode, Leaf> node) => node.IsT1;
        private static bool IsLeaf(OneOf<AndNode, OrNode, Leaf> node) => node.IsT2;
        private static bool IsTrueLeaf(OneOf<AndNode, OrNode, Leaf> node) =>
            node.IsT2 && !node.AsT2.Value.Any(char.IsDigit);

        private static Dictionary<int, OneOf<AndNode, OrNode, Leaf>> BuildRules(IEnumerable<string> lines)
        {
            var rc = new Dictionary<int, OneOf<AndNode, OrNode, Leaf>>();
            foreach (var line in lines)
            {
                var lineParts = line.Replace("\"", string.Empty).Split(':', StringSplitOptions.TrimEntries);
                var ruleNumber = int.Parse(lineParts[0]);
                var rule = lineParts[1];
                if (rule.Length == 1) rc.Add(ruleNumber, new Leaf(rule));
                else if (!rule.Contains('|')) rc.Add(ruleNumber, new AndNode(LeafArray(rule)));
                else
                {
                    var parts = rule.Split('|', StringSplitOptions.TrimEntries);
                    rc.Add(ruleNumber, new OrNode(new[]
                    {
                        (OneOf<AndNode, OrNode, Leaf>) new AndNode(LeafArray(parts[0])),
                        (OneOf<AndNode, OrNode, Leaf>) new AndNode(LeafArray(parts[1]))
                    }));
                }
            }
            return rc;
        }

        private static OneOf<AndNode, OrNode, Leaf>[] LeafArray(string s) =>
            s.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)
                .Select(x => (OneOf<AndNode, OrNode, Leaf>) new Leaf(x.ToString())).ToArray();
    }

    internal record OrNode (OneOf<AndNode, OrNode, Leaf>[] Sequence);
    internal record AndNode (OneOf<AndNode, OrNode, Leaf>[] Sequence);
    internal record Leaf (string Value);
}