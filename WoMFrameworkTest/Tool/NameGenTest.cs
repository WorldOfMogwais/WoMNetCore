using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Random;
using Xunit;

namespace WoMFrameworkTest.Tool
{
    public class NameGenTest
    {
        [Fact]
        public void GenerateNameTest()
        {
            var hexValue = 
                new HexValue(
                    new Shift(0,
                    1530914381,
                    "32ad9e02792599dfdb6a9d0bc0b924da23bd96b1b7eb4f0a68",
                    7234,
                    "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                    2,
                    "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                    1.00m,
                    0.0001m));
            Assert.Equal("Vihiryne", NameGen.GenerateName(hexValue));
        }
    }
}
