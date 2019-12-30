namespace WoMFrameworkTest.Tool
{
    using GoRogue;
    using GoRogue.MapViews;
    using Troschuetz.Random;
    using WoMFramework.Game.Interaction;
    using WoMFramework.Game.Random;
    using WoMFramework.Tool;
    using Xunit;
 
    [Collection("SystemInteractionFixture")]
    public class ArrayMapHelperTest
    {
        private IGenerator random;

        public ArrayMapHelperTest()
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

            var pMap = new ArrayMap<bool>(20, 20);

            map[5, 5] = true;


            map[10, 10] = true;

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

            Assert.True(ArrayMapHelper.TryGetFirstPatternMatch(map, pMap, pat1, out Coord startCoord1));
            Assert.Equal(4, startCoord1.X);
            Assert.Equal(4, startCoord1.Y);

            pMap[4, 4] = true;
            pMap[5, 4] = true;
            pMap[6, 4] = true;
            pMap[4, 5] = true;
            pMap[5, 5] = true;
            pMap[6, 5] = true;
            pMap[4, 6] = true;
            pMap[5, 6] = true;
            pMap[6, 6] = true;


            Assert.True(ArrayMapHelper.TryGetFirstPatternMatch(map, pMap, pat1, out Coord startCoord2));
            Assert.Equal(9, startCoord2.X);
            Assert.Equal(9, startCoord2.Y);

        }

        [Fact]
        public void RotateArrayMap()
        {
            ArrayMap<bool> arrayMap1 = new ArrayMap<bool>(4,4);
            arrayMap1[0, 0] = true;

            Assert.True(arrayMap1[0, 0]);

            var arrayMap90deg1 = ArrayMapHelper.RotateArrayMap(arrayMap1, RotateType.DEG90);

            Assert.True(arrayMap90deg1[0, 3]);

            var arrayMap180deg1 = ArrayMapHelper.RotateArrayMap(arrayMap1, RotateType.DEG180);

            Assert.True(arrayMap180deg1[3, 3]);

            var arrayMap270deg1 = ArrayMapHelper.RotateArrayMap(arrayMap1, RotateType.DEG270);

            Assert.True(arrayMap270deg1[3, 0]);


            ArrayMap<bool> arrayMap2 = new ArrayMap<bool>(2, 3);
            arrayMap2[0, 0] = true;

            Assert.True(arrayMap2[0, 0]);

            var arrayMap90deg2 = ArrayMapHelper.RotateArrayMap(arrayMap2, RotateType.DEG90);

            Assert.True(arrayMap90deg2[0, 1]);

            var arrayMap180deg2 = ArrayMapHelper.RotateArrayMap(arrayMap2, RotateType.DEG180);

            Assert.True(arrayMap180deg2[1, 2]);

            var arrayMap270deg2 = ArrayMapHelper.RotateArrayMap(arrayMap2, RotateType.DEG270);

            Assert.True(arrayMap270deg2[2, 0]);
        }

        [Fact]
        public void EncodeBase64Test()
        {
            ArrayMap<bool> arrayMap1 = new ArrayMap<bool>(4, 4);
            Assert.Equal("X4Y4N000-AAA=", ArrayMapHelper.EncodeBase64(arrayMap1));
        }

        [Fact]
        public void DecodeBase64Test()
        {
            ArrayMap<bool> arrayMap1 = ArrayMapHelper.DecodeBase64("X4Y4N000-AAA=");
            Assert.Equal(4, arrayMap1.Width);
            Assert.Equal(4, arrayMap1.Height);
            foreach(var pos in arrayMap1.Positions())
            {
                Assert.False(arrayMap1[pos]);
            }          
        }
    }
}
