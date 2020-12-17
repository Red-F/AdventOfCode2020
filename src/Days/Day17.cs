using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Days
{
    [Day(2020, 17)]
    public class Day17 : BaseDay
    {
        public override string PartOne(string input)
        {
            var state = CreateInitial3DGrid(input);
            for (var i = 0; i < 6; i++) state = Cycle(state);
            return state.Count.ToString();
        }

        public override string PartTwo(string input)
        {
            var state = CreateInitial4DGrid(input);
            for (var i = 0; i < 6; i++) state = HyperCycle(state);
            return state.Count.ToString();
        }

        private static HashSet<Point3D> Cycle(IReadOnlySet<Point3D> state)
        {
            var newState = new HashSet<Point3D>();
            foreach (var thisCube in ActiveCubesAndNeighbours(state))
            {
                var neighbours = Neighbours(thisCube);
                var activeNeighbours = neighbours.Count(state.Contains);
                if (!state.Contains(thisCube))
                {
                    if (activeNeighbours == 3) newState.Add(thisCube);
                }
                else if (activeNeighbours == 2 || activeNeighbours == 3) newState.Add(thisCube);
            }
            return newState;
        }

        private static HashSet<Point4D> HyperCycle(IReadOnlySet<Point4D> state)
        {
            var newState = new HashSet<Point4D>();
            foreach (var thisHyperCube in ActiveHyperCubesAndNeighbours(state))
            {
                var neighbours = HyperNeighbours(thisHyperCube);
                var activeNeighbours = neighbours.Count(state.Contains);
                if (!state.Contains(thisHyperCube))
                {
                    if (activeNeighbours == 3) newState.Add(thisHyperCube);
                }
                else if (activeNeighbours == 2 || activeNeighbours == 3) newState.Add(thisHyperCube);
            }
            return newState;
        }

        private static IEnumerable<Point3D> ActiveCubesAndNeighbours(IEnumerable<Point3D> state)
        {
            var cubeSet = new HashSet<Point3D>();
            foreach (var cube in state) cubeSet.AddRange(Neighbours(cube, includeMySelf: true));
            return cubeSet;
        }

        private static IEnumerable<Point4D> ActiveHyperCubesAndNeighbours(IEnumerable<Point4D> state)
        {
            var hyperCubeSet = new HashSet<Point4D>();
            foreach (var cube in state) hyperCubeSet.AddRange(HyperNeighbours(cube, includeMySelf: true));
            return hyperCubeSet;
        }

        private static IEnumerable<Point3D> Neighbours(Point3D point, bool includeMySelf = false)
        {
            var rc = new HashSet<Point3D>();
            for (var x = point.X - 1; x <= point.X + 1; x++)
            for (var y = point.Y - 1; y <= point.Y + 1; y++)
            for (var z = point.Z - 1; z <= point.Z + 1; z++)
                rc.Add(new Point3D(x, y, z));
            if (!includeMySelf) rc.Remove(point);
            return rc;
        }

        private static IEnumerable<Point4D> HyperNeighbours(Point4D point, bool includeMySelf = false)
        {
            var rc = new HashSet<Point4D>();
            for (var x = point.X - 1; x <= point.X + 1; x++)
            for (var y = point.Y - 1; y <= point.Y + 1; y++)
            for (var z = point.Z - 1; z <= point.Z + 1; z++)
            for (var t = point.T - 1; t <= point.T + 1; t++)
                rc.Add(new Point4D(x, y, z, t));
            if (!includeMySelf) rc.Remove(point);
            return rc;
        }

        private static HashSet<Point3D> CreateInitial3DGrid(string input)
        {
            var rc = new HashSet<Point3D>();
            var grid = input.CreateCharGrid();
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
                if (grid[x, y] == '#')
                    rc.Add(new Point3D(x, y, 0));
            return rc;
        }

        private static HashSet<Point4D> CreateInitial4DGrid(string input)
        {
            var rc = new HashSet<Point4D>();
            var grid = input.CreateCharGrid();
            for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
                if (grid[x, y] == '#')
                    rc.Add(new Point4D(x, y, 0, 0));
            return rc;
        }
    }
}