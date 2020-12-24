using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    [Day(2020, 24)]
    public class Day24 : BaseDay
    {
        // learned a lot from https://catlikecoding.com/unity/tutorials/hex-map/part-1/
        public override string PartOne(string input) =>
            InitialiseTiles(input.Lines().ParseHexDirections()).Count.ToString();

        public override string PartTwo(string input) => Flip(input.Lines().ParseHexDirections()).ToString();

        private static long Flip(IEnumerable<string[]> directionsList)
        {
            var blackTiles = InitialiseTiles(directionsList);
            for (var i = 0; i < 100; i++) blackTiles = FlipTiles(new HashSet<(int x, int y)>(blackTiles.ToArray()));
            return blackTiles.Count;
        }

        private static List<(int x, int y)> InitialiseTiles(IEnumerable<string[]> directionsList)
        {
            var blackTiles = new List<(int x, int y)>();
            foreach (var directions in directionsList)
            {
                (int x, int y) position = (0, 0);
                position = directions.Select(direction => HexDirections[direction]).Aggregate(position,
                    (current, step) => (current.x + step.x, current.y + step.y));

                if (blackTiles.Contains(position)) blackTiles.Remove(position);
                else blackTiles.Add(position);
            }
            return blackTiles;
        }

        private static List<(int x, int y)> FlipTiles(HashSet<(int x, int y)> tiles)
        {
            var newBlackTiles = new List<(int x, int y)>();
            var tilesToCheck = new HashSet<(int x, int y)>();
            tiles.ForEach(x => HexNeighbours(x, true).ForEach(t => tilesToCheck.Add(t)));
            foreach (var tile in tilesToCheck)
            {
                var blackNeighbours = HexNeighbours(tile).Count(tiles.Contains);
                if (tiles.Contains(tile))
                {
                    if (blackNeighbours == 1 || blackNeighbours == 2) newBlackTiles.Add(tile); 
                }
                else
                {
                    if (blackNeighbours == 2) newBlackTiles.Add(tile);
                }
            }
            return newBlackTiles;
        }
        
        private static HashSet<(int x, int y)> HexNeighbours((int x, int y) tile, bool includeMySelf = false)
        {
            var rc = new HashSet<(int x, int y)>();
            if (includeMySelf) rc.Add(tile);
            foreach (var (_, (x, y)) in HexDirections) rc.Add((tile.x + x, tile.y + y));
            return rc;
        }

        private static readonly Dictionary<string, (int x, int y)> HexDirections = new()
        {
            {"e", (1, 0)}, {"se", (1, -1)}, {"sw", (0, -1)}, {"w", (-1, 0)}, {"nw", (-1, 1)}, {"ne", (0, 1)}
        };
    }
    
    internal static class Day24Extensions
    {
        public static IEnumerable<string[]> ParseHexDirections(this IEnumerable<string> data)
        {
            var instructions = data.ToArray();
            var r = new Regex(@"((ne)|(nw)|(se)|(sw)|(e)|(w))");
            var rc = new List<string[]>();
            for (var i = 0; i < instructions.Count(); i++)
                rc.Add(r.Matches(instructions[i]).Select(x => x.ToString()).ToArray());
            return rc;
        }
    }
}
