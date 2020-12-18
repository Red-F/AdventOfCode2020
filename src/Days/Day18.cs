using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    [Day(2020, 18)]
    public class Day18 : BaseDay
    {
        public override string PartOne(string input)
        {
            var data = input.Lines();
            return data.Sum(s => EvaluateExpression(s, EvaluateOne)).ToString();
        }

        public override string PartTwo(string input)
        {
            var data = input.Lines();
            return data.Sum(s => EvaluateExpression(s, EvaluateTwo)).ToString();
        }

        private static long EvaluateExpression(string input, Func<string, string> solver)
        {
            var elements = input.Replace(" ", string.Empty);
            while (elements.Any(c => c == '('))
            {
                var innerExpression = string.Concat(elements.Take(elements.IndexOf(')') + 1));
                innerExpression = string.Concat(innerExpression.Skip(innerExpression.LastIndexOf('(')));
                elements = ReplaceFirstMatch(elements, innerExpression!, solver(innerExpression));
            }
            return long.Parse(solver(elements));
        }

        private static string EvaluateOne(string expression)
        {
            var r = new Regex(@"[+*][0-9]+");
            var expressionWithoutParenthesis = expression.Replace("(", string.Empty).Replace(")", string.Empty);
            var rc = long.Parse(new Regex(@"[0-9]+").Match(expressionWithoutParenthesis).Value);
            var terms = r.Matches(expressionWithoutParenthesis);
            for (var i = 0; i < terms.Count; i++)
            {
                if (terms[i].Value[0] == '+') rc += long.Parse(terms[i].Value.Substring(1));
                else rc *= long.Parse(terms[i].Value.Substring(1));
            }
            return rc.ToString();
        }

        private static string EvaluateTwo(string expression)
        {
            var expressionWithoutParenthesis = expression.Replace("(", string.Empty).Replace(")", string.Empty);
            while (expressionWithoutParenthesis.Contains('+'))
            {
                var m = new Regex(@"([0-9]+)\+([0-9]+)").Match(expressionWithoutParenthesis);
                var intermediate = long.Parse(m.Groups[1].Value) + long.Parse(m.Groups[2].Value);
                expressionWithoutParenthesis =
                    expressionWithoutParenthesis.Replace(m.Groups[0].Value, intermediate.ToString());
            }
            while (expressionWithoutParenthesis.Contains('*'))
            {
                var m = new Regex(@"([0-9]+)\*([0-9]+)").Match(expressionWithoutParenthesis);
                var intermediate = long.Parse(m.Groups[1].Value) * long.Parse(m.Groups[2].Value);
                expressionWithoutParenthesis = ReplaceFirstMatch(expressionWithoutParenthesis, m.Groups[0].Value,
                    intermediate.ToString());
            }
            return expressionWithoutParenthesis;
        }

        private static string ReplaceFirstMatch(string theString, string toFind, string replacement)
        {
            var pos = theString.IndexOf(toFind, StringComparison.Ordinal);
            return theString.Remove(pos, toFind.Length).Insert(pos, replacement);
        }
    }
}