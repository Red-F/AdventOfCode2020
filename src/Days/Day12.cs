using System;
using System.Collections.Generic;

namespace AdventOfCode.Days
{
    [Day(2020, 12)]
    public class Day12 : BaseDay
    {
        public override string PartOne(string input) => ExecutePartOne(input.ParseStrings(ParseInstruction)).ToString();

        public override string PartTwo(string input) => ExecutePartTwo(input.ParseStrings(ParseInstruction)).ToString();

        private static int ExecutePartOne(IEnumerable<(char action, int value)> instructions)
        {
            var distances = new Dictionary<char, int> {{'N', 0}, {'E', 0}, {'W', 0}, {'S', 0}};
            var directions = new[] {'N', 'E', 'S', 'W'};
            var facing = 1;
            foreach (var (action, value) in instructions)
                switch (action)
                {
                    case 'N':
                    case 'E':
                    case 'S':
                    case 'W':
                        distances[action] += value;
                        break;
                    case 'F':
                        distances[directions[facing]] += value;
                        break;
                    case 'R':
                        facing = (facing + value / 90) % 4;
                        break;
                    case 'L':
                        facing = (facing + 4 - value / 90) % 4;
                        break;
                }

            return Math.Abs(distances['N'] - distances['S']) + Math.Abs(distances['E'] - distances['W']);
        }

        private static long ExecutePartTwo(IEnumerable<(char action, int value)> instructions)
        {
            var waypointDistances = new Dictionary<char, long> {{'N', 1}, {'E', 10}, {'W', 0}, {'S', 0}};
            var shipDistances = new Dictionary<char, long> {{'N', 0}, {'E', 0}, {'W', 0}, {'S', 0}};
            foreach (var (action, value) in instructions)
                switch (action)
                {
                    case 'N':
                    case 'E':
                    case 'S':
                    case 'W':
                        waypointDistances[action] += value;
                        break;
                    case 'F':
                        foreach (var (key, i) in waypointDistances)
                        {
                            shipDistances[key] += value * i;
                        }

                        break;
                    case 'R':
                        for (var i = 0; i < value / 90; i++)
                        {
                            var oldNorthRight = waypointDistances['N'];
                            waypointDistances['N'] = waypointDistances['W'];
                            waypointDistances['W'] = waypointDistances['S'];
                            waypointDistances['S'] = waypointDistances['E'];
                            waypointDistances['E'] = oldNorthRight;
                        }

                        break;
                    case 'L':
                        for (var i = 0; i < value / 90; i++)
                        {
                            var oldNorthLeft = waypointDistances['N'];
                            waypointDistances['N'] = waypointDistances['E'];
                            waypointDistances['E'] = waypointDistances['S'];
                            waypointDistances['S'] = waypointDistances['W'];
                            waypointDistances['W'] = oldNorthLeft;
                        }

                        break;
                }

            return Math.Abs(shipDistances['N'] - shipDistances['S']) +
                   Math.Abs(shipDistances['E'] - shipDistances['W']);
        }

        private static (char action, int value) ParseInstruction(string arg)
        {
            return (arg[0], int.Parse(arg.Substring(1)));
        }
    }
}