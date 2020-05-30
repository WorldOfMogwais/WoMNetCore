namespace WoMFrameworkTest.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using WoMFramework.Game.Interaction;
    using WoMFramework.Game.Model;
    using WoMFramework.Game.Model.Mogwai;
    using WoMFramework.Tool;
    using Xunit;

    [Collection("SystemInteractionFixture")]
    public class HomeTownTest
    {
        public const string MogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";

        public Mogwai Mogwai;

        public HomeTownTest()
        {
            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(MogAddress));

            var creation = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            Assert.True(creation.History != null);

            var shifts = new Dictionary<long, Shift>
            {
                {2000, creation}
            };
            Mogwai = new Mogwai(MogAddress, shifts);
        }

        [Fact]
        public void ShopTest()
        {
            Assert.Equal(10, Mogwai.HomeTown.Shop.Inventory.Count);
            Assert.Equal(99.92000000000002, Mogwai.HomeTown.Shop.Inventory.Sum(p => p.Cost));
            Assert.True(Mogwai.HomeTown.Shop.Inventory.OfType<Armor>().Any());
            Assert.Equal("Klar", Mogwai.HomeTown.Shop.Inventory.OfType<Armor>().First().Name);
            Assert.True(Mogwai.HomeTown.Shop.Inventory.OfType<Weapon>().Any());
            Assert.Equal("Starknife", Mogwai.HomeTown.Shop.Inventory.OfType<Weapon>().First().Name);
            Assert.True(Mogwai.HomeTown.Shop.Inventory.OfType<Potion>().Any());
            Assert.Equal("Cure Light Wounds", Mogwai.HomeTown.Shop.Inventory.OfType<Potion>().First().Name);

        }
    }
}
