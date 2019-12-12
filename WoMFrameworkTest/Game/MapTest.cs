namespace WoMFrameworkTest.Game
{
    using GoRogue;
    using GoRogue.MapViews;
    using System.Collections.Generic;
    using Troschuetz.Random;
    using WoMFramework.Game.Generator.Dungeon;
    using WoMFramework.Game.Interaction;
    using WoMFramework.Game.Random;
    using WoMFramework.Tool;
    using Xunit;

    [Collection("SystemInteractionFixture")]
    public class MapTest
    {
        private IGenerator random;

        public MapTest()
        {
            var shift = new Shift(0,
               1530914381,
               "32ad9e02792599dfdb6a9d0bc0b924da23bd96b1b7eb4f0a68",
               7234,
               "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
               2,
               "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
               1.00m,
               0.0001m);
            random = new Dice(shift).GetRandomGenerator();
        }

        [Fact]
        public void TryGetFirstPatternMatchTest()
        {
            var map = new ArrayMap<bool>(20, 20);
            Assert.False(map[0, 0]);

            map[5, 5] = true;


            var pat1 = new ArrayMap<bool>(3, 3);

            pat1[0, 0] = false;
            pat1[1, 0] = false;
            pat1[2, 0] = false;
            pat1[0, 1] = false;
            pat1[1, 1] = true;
            pat1[2, 1] = false;
            pat1[0, 2] = false;
            pat1[1, 2] = false;
            pat1[2, 2] = false;

            Assert.True(Map.TryGetFirstPatternMatch(map, pat1, out Coord startCoord));
            Assert.Equal(4, startCoord.X);
            Assert.Equal(4, startCoord.Y);

        }
    }
}
