using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Days
{
    [Day(2020, 20)]
    public class Day20 : BaseDay
    {
        public override string PartOne(string input)
        {
            var tiles = ParseTiles(input);
            var allOrientations = AllOrientationsForAllTiles(tiles);
            return allOrientations
                .Where(tile =>
                    tile.Value.Take(4).Count(s => !MatchesSide(s.Row(0), allOrientations, tile.Key)) == 2)
                .Select(t => t.Key).Aggregate(1L, (l, id) => l * id).ToString();
        }

        public override string PartTwo(string input)
        {
            var tiles = ParseTiles(input);
            var image = CreateImage(tiles);
            var imageOrientations = Orientations(image);
            foreach (var imageOrientation in imageOrientations)
            {
                var seaMonsters = CountSeaMonsters(imageOrientation);
                if (seaMonsters > 0) return (imageOrientation.Count('#') - seaMonsters * 15).ToString();
            }
            throw new Exception("Should not get here");
        }

        private static int CountSeaMonsters(char[,] image)
        {
            var rc = 0;
            for (var y = 0; y < image.GetLength(1) - 3; y++)
            for (var x = 0; x < image.GetLength(0) - 20; x++)
                if (IsSeaMonster(image, x, y)) rc++;
            return rc;
        }

        private static readonly (int x, int y)[] SeaMonster = new[]
        {
            (0, 1), (1, 2), (4, 2), (5, 1), (6, 1), (7, 2), (10, 2), (11, 1), (12, 1), (13, 2), (16, 2), (17, 1),
            (18, 0), (18, 1), (19, 1)
        };

        private static bool IsSeaMonster(char[,] image, int x, int y) =>
            SeaMonster.All(p => image[x + p.x, y + p.y] == '#');

        private static char[,] CreateImage(Dictionary<int, char[,]> tiles)
        {
            var sizeInTiles = (int) Math.Sqrt(tiles.Count);
            var image = SolvePositions(tiles, sizeInTiles);
            return GlueTogether(image);
        }

        private static char[,] GlueTogether(char[,][,] image)
        {
            var innerWidth = image[0, 0].GetLength(0) - 2;
            var innerHeight = image[0, 0].GetLength(1) - 2;
            var totalWidth = innerWidth * image.GetLength(0);
            var totalHeight = innerHeight * image.GetLength(1);
            var rc = new char[totalWidth, totalHeight];
            for (var y = 0; y < image.GetLength(1); y++)
            for (var x = 0; x < image.GetLength(0); x++)
            for (var yi = 1; yi < innerHeight + 1; yi++)
            for (var xi = 1; xi < innerWidth + 1; xi++)
            {
                var posX = x * innerWidth + xi - 1;
                var posY = y * innerHeight + yi - 1;
                rc[posX, posY] = image[x, y][xi, yi];
            }
            return rc;
        }

        private static char[,][,] SolvePositions(Dictionary<int, char[,]> tiles, int sizeInTiles)
        {
            var image = new char[sizeInTiles, sizeInTiles][,];
            var allOrientations = AllOrientationsForAllTiles(tiles);
            var tlCorner = FindTopLeftCorner(allOrientations);
            image[0, 0] = tlCorner.map;
            allOrientations.Remove(tlCorner.id);
            for (var y = 0; y < sizeInTiles; y++)
            for (var x = 0; x < sizeInTiles; x++)
            {
                if (x == 0 && y == 0) continue;
                var (id, map) = FindImageTile(x, y, image, allOrientations);
                image[x, y] = map;
                allOrientations.Remove(id);
            }
            return image;
        }

        private static (int id, char[,] map) FindImageTile(int x, int y, char[,][,] image,
            Dictionary<int, List<char[,]>> allOrientations)
        {
            (string top, string left) match = ("na", "na");
            if (y != 0) match.top = image[x, y - 1].BottomRow();
            if (x != 0) match.left = image[x - 1, y].RightColumn();
            foreach (var (id, orientations) in allOrientations)
            foreach (var orientation in orientations.Where(orientation =>
                Matches(orientation, match, allOrientations, id)))
                return (id, orientation);
            throw new Exception("Should not get here");
        }

        private static bool Matches(char[,] orientation, (string top, string left) match,
            Dictionary<int, List<char[,]>> allOrientations, int id)
        {
            if (match.left == "na" && MatchesSide(orientation.LeftColumn(), allOrientations, id)) return false;
            if (match.top == "na" && MatchesSide(orientation.TopRow(), allOrientations, id)) return false;
            if (match.left != "na" && orientation.LeftColumn() != match.left) return false;
            return match.top == "na" || orientation.TopRow() == match.top;
        }

        private static (int id, char[,] map) FindTopLeftCorner(Dictionary<int, List<char[,]>> allOrientations)
        {
            foreach (var id in allOrientations)
            foreach (var orientation in id.Value.Where(orientation =>
                !MatchesSide(orientation.TopRow(), allOrientations, id.Key) &&
                !MatchesSide(orientation.LeftColumn(), allOrientations, id.Key)))
                return (id.Key, orientation);
            throw new Exception("Should not get here");
        }

        private static bool MatchesSide(string side, Dictionary<int, List<char[,]>> mapOrientations, int id)
        {
            foreach (var (currentId, orientations) in mapOrientations)
            {
                if (currentId == id) continue;
                if (orientations.Any(orientation => orientation.Row(0) == side)) return true;
            }
            return false;
        }

        private static Dictionary<int, List<char[,]>> AllOrientationsForAllTiles(Dictionary<int, char[,]> tiles)
        {
            var rc = new Dictionary<int, List<char[,]>>();
            foreach (var (id, map) in tiles) rc.Add(id, Orientations(map).ToList());
            return rc;
        }

        private static Dictionary<int, char[,]> ParseTiles(string input)
        {
            var groups = input.LineGroups(Environment.NewLine + Environment.NewLine);
            var rc = new Dictionary<int, char[,]>();
            foreach (var group in groups)
            {
                var id = int.Parse(
                    group.Lines().First().Split(new[] {' ', ':'}, StringSplitOptions.RemoveEmptyEntries)[1]);
                var map = string.Join(Environment.NewLine, group.Lines().Skip(1)).CreateCharGrid();
                rc.Add(id, map);
            }
            return rc;
        }

        private static IEnumerable<char[,]> Orientations(char[,] map)
        {
            yield return map;
            yield return map.RotateRight(1);
            yield return map.RotateRight(2);
            yield return map.RotateRight(3);
            yield return map.Flip();
            yield return map.Flip().RotateRight(1);
            yield return map.Flip().RotateRight(2);
            yield return map.Flip().RotateRight(3);
        }
    }

    internal static class Extensions
    {
        internal static string LeftColumn(this char[,] map) => map.Column(0);
        internal static string RightColumn(this char[,] map) => map.Column(map.GetUpperBound(0));
        internal static string TopRow(this char[,] map) => map.Row(0);
        internal static string BottomRow(this char[,] map) => map.Row(map.GetUpperBound(1));
        private static string Column(this char[,] map, int column)
        {
            var sb = new StringBuilder();
            for (var y = 0; y < map.GetLength(1); y++) sb.Append(map[column, y]);
            return sb.ToString();
        }
        internal static string Row(this char[,] map, int row)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < map.GetLength(0); x++) sb.Append(map[x, row]);
            return sb.ToString();
        }
        internal static char[,] RotateRight(this char[,] map, int count)
        {
            var rc = map;
            for (var i = 0; i < count; i++) rc = rc.RotateRight();
            return rc;
        }
        private static char[,] RotateRight(this char[,] map)
        {
            var width = map.GetLength(0);
            var rc = new char[width, map.GetLength(1)];
            for (var y = 0; y < map.GetLength(1); y++)
            for (var x = 0; x < width; x++)
                rc[width - y - 1, x] = map[x, y];
            return rc;
        }
        internal static char[,] Flip(this char[,] map)
        {
            var height = map.GetLength(1);
            var rc = new char[map.GetLength(0), height];
            for (var y = 0; y < height; y++)
            for (var x = 0; x < map.GetLength(0); x++)
                rc[x, height - y - 1] = map[x, y];
            return rc;
        }
    }
}