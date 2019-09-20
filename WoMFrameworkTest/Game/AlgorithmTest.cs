namespace WoMFrameworkTest.Game
{
    using GoRogue;
    using GoRogue.MapViews;
    using System.Linq;
    using Xunit;

    [Collection("SystemInteractionFixture")]
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

            var current = new Coord(1, 1);

            Coord[] neighbours = WoMFramework.Game.Algorithms.GetReachableNeighbors(testMap, current);

            Assert.Equal(5, neighbours.Length);
            Assert.DoesNotContain(new Coord(2, 2), neighbours);
            Assert.DoesNotContain(new Coord(1, 2), neighbours);
            Assert.DoesNotContain(new Coord(0, 2), neighbours);
        }

        [Fact]
        public void FOVTest()
        {
            var testMap = new ArrayMap<bool>(5, 5);
            foreach (Coord coord in testMap.Positions())
                testMap[coord] = true;

            var centre = new Coord(2, 2);
            foreach (Coord coord in new RadiusAreaProvider(centre, 2, Radius.CIRCLE).CalculatePositions())
                testMap[coord] = false;
            foreach (Coord coord in new RadiusAreaProvider(centre, 1, Distance.EUCLIDEAN).CalculatePositions())
                testMap[coord] = true;

            var resMap = new ArrayMap<bool>(5, 5);
            foreach (Coord coord in testMap.Positions())
                resMap[coord] = testMap[coord];

            var FOV = new FOV(resMap);

            FOV.Calculate(centre, 1, Distance.EUCLIDEAN);

            Coord[] calculated = FOV.CurrentFOV.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
            Assert.True(calculated.SequenceEqual(new[]
                {new Coord(1, 2), new Coord(2, 1), new Coord(2, 2), new Coord(2, 3), new Coord(3, 2)}));
        }
    }
}
