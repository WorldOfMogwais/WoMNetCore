namespace WoMFrameworkTest.Tool
{
    using GoRogue.MapViews;
    using WoMFramework.Tool;
    using Xunit;
 
    [Collection("SystemInteractionFixture")]
    public class ArrayMapHelperTest
    {
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
    }
}
