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
    }
}
