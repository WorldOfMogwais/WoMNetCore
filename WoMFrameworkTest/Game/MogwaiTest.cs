using System.Collections.Generic;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class MogwaiTest
    {

        public Mogwai Mogwai { get;  }

        public MogwaiTest()
        {
            var address = "MJHYMxu2kyR1Bi4pYwktbeCM7yjZyVxt2i";
            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(address));

            Mogwai = new Mogwai(address, new Dictionary<long, Shift>
            {
                {1001, new Shift(0, 1530914381, pubMogAddressHex,
                    1001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                    2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                    1.00m,
                    0.0001m)}
            });
        }

        [Fact]
        public void MogwaiBasicTest()
        {
            Assert.Equal("Heckaece", Mogwai.Name);
        }

        [Fact]
        public void MogwaiInventory()
        {
            Assert.Equal(0, Mogwai.Inventory.Count);
        }

        [Fact]
        public void MogwaiWeapon()
        {
            Assert.Equal(1, Mogwai.Equipment.WeaponSlots.Count);
        }

        [Fact]
        public void MogwaiArmor()
        {
            Assert.Equal(3, Mogwai.Equipment.ArmorBonus);
        }
    }
}