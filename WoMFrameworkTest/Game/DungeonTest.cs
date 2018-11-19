using System.Collections.Generic;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class DungeonTest
    {
        [Fact]
        public void SimpleDungeonTest()
        {
            var address = "MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i";
            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(address));
            var shifts = new Dictionary<long, Shift>
            {
                {1001, new Shift(0, 1530914381, pubMogAddressHex,
                                1001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                                1.00m,
                                0.0001m)},

                {1002, new Shift(1, 1535295740, pubMogAddressHex,
                               1002, "0000000033dbfc3cc9f3671ba28b41ecab6f547219bb43174cc97bf23269fa88",
                               1, "db5639553f9727c42f80c22311bd8025608edcfbcfc262c0c2afe9fc3f0bcb29",
                               0.01040003m,
                               0.00001002m)}
            };

            var mogwai = new Mogwai("MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i", shifts);
            //mogwai.EnterSimpleDungeon();

        }
    }
}
