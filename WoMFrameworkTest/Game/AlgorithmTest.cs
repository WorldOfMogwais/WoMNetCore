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
            ArrayMap<bool> testMap = new ArrayMap<bool>(3, 3);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    testMap[i, j] = true;

            testMap[0, 2] = false;
            testMap[1, 2] = false;

            Coord current = Coord.Get(1, 1);

            Coord[] neighbours = WoMFramework.Game.Algorithms.GetReachableNeighbours(testMap, current);

            Assert.Equal(5, neighbours.Length);
            Assert.DoesNotContain(Coord.Get(2, 2), neighbours);
            Assert.DoesNotContain(Coord.Get(1, 2), neighbours);
            Assert.DoesNotContain(Coord.Get(0, 2), neighbours);
        }

        [Fact]
        public void FOVTest()
        {
            var testMap = new ArrayMap<bool>(5, 5);
            foreach (var coord in testMap.Positions())
                testMap[coord] = true;

            var centre = Coord.Get(2, 2);
            foreach (var coord in new RadiusAreaProvider(centre, 2, Radius.CIRCLE).CalculatePositions())
                testMap[coord] = false;
            foreach (var coord in new RadiusAreaProvider(centre, 1, Distance.EUCLIDEAN).CalculatePositions())
                testMap[coord] = true;

            var resMap = new ArrayMap<double>(5, 5);
            foreach (var coord in testMap.Positions())
                resMap[coord] = testMap[coord] ? 0 : 1.0;

            var FOV = new FOV(resMap);

            FOV.Calculate(centre, 1, Distance.EUCLIDEAN);

            var calculated = FOV.CurrentFOV.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
            Assert.True(calculated.SequenceEqual(new []
                {Coord.Get(1, 2), Coord.Get(2, 1), Coord.Get(2, 2), Coord.Get(2, 3), Coord.Get(3, 2)}));
        }
    }
}
