using WoMFramework.Game.Interaction;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Tool
{
    public class HexHashUtilTest
    {
        [Fact]
        public void HashSha256Test()
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

            var bytes = HexHashUtil.StringToByteArray(shift.AdHex);
            var hashedBytes = HexHashUtil.HashSha256(bytes);
            var hash = HexHashUtil.ByteArrayToString(hashedBytes);
            Assert.Equal("bcd35d9f6de167fada34bfeb0c59d2b1a55974b9460c54f42dfff2e86cf8c58b", hash);
        }
    }
}
