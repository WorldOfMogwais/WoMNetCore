using System.Linq;
using GoRogue;
using GoRogue.MapViews;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class AlgorithmTest
    {
        [Fact]
        public void NeighbourTest()
        {
            var testMap = new ArrayMap<bool>(3, 3);
            for (var i = 0; i < 3; i++)
                for (var j = 0; j < 3; j++)
                    testMap[i, j] = true;

            testMap[0, 2] = false;
            testMap[1, 2] = false;

            var current = Coord.ToCoord(1, 1);

            var neighbours = WoMFramework.Game.Algorithms.GetReachableNeighbours(testMap, current);

            Assert.Equal(5, neighbours.Length);
            Assert.DoesNotContain(Coord.ToCoord(2, 2), neighbours);
            Assert.DoesNotContain(Coord.ToCoord(1, 2), neighbours);
            Assert.DoesNotContain(Coord.ToCoord(0, 2), neighbours);
        }

        [Fact]
        public void FOVTest()
        {
            var testMap = new ArrayMap<bool>(5, 5);

            foreach (var coord in testMap.Positions())
                testMap[coord] = true;

            var centre = Coord.ToCoord(2, 2);

            foreach (var coord in new RadiusAreaProvider(centre, 2, Radius.CIRCLE).CalculatePositions())
                testMap[coord] = false;

            foreach (var coord in new RadiusAreaProvider(centre, 1, Distance.EUCLIDEAN).CalculatePositions())
                testMap[coord] = true;

            var resMap = new ArrayMap<bool>(5, 5);
            foreach (var coord in testMap.Positions())
                resMap[coord] = !testMap[coord];

            var FOV = new FOV(resMap);

            FOV.Calculate(centre, 1, Distance.EUCLIDEAN);

            var calculated = FOV.CurrentFOV.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
            Assert.True(calculated.SequenceEqual(new []
                {Coord.ToCoord(1, 2), Coord.ToCoord(2, 1), Coord.ToCoord(2, 2), Coord.ToCoord(2, 3), Coord.ToCoord(3, 2)}));
        }
    }
}
