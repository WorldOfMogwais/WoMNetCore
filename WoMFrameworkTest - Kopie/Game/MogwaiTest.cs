using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
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
            Assert.Empty(Mogwai.Inventory);
        }

        [Fact]
        public void MogwaiWeapon()
        {
            Assert.Single(Mogwai.Equipment.WeaponSlots);
        }

        [Fact]
        public void MogwaiArmor()
        {
            Assert.Equal(3, Mogwai.Equipment.ArmorBonus);
        }

        [Fact]
        public void MogwaiMagicItem()
        {
            var ringOfMogwan = MagicItems.BoneOfMogwan();
            var ringOfTheBear = MagicItems.RingOfTheBear();

            Assert.Empty(Mogwai.Inventory);

            Mogwai.AddToInventory(ringOfMogwan);
            Mogwai.AddToInventory(ringOfTheBear);

            Assert.Equal(2, Mogwai.Inventory.Count);
            Assert.Equal(0, Mogwai.StrengthMod);
            Assert.Equal(0, Mogwai.DexterityMod);
            Assert.Equal(0, Mogwai.ConstitutionMod);
            Assert.Equal(0, Mogwai.InteligenceMod);
            Assert.Equal(1, Mogwai.WisdomMod);
            Assert.Equal(3, Mogwai.CharismaMod);

            Mogwai.EquipeItem(SlotType.Ring, ringOfMogwan, 0);

            Assert.Equal(1, Mogwai.Inventory.Count);
            Assert.Equal(1, Mogwai.StrengthMod);
            Assert.Equal(1, Mogwai.DexterityMod);
            Assert.Equal(1, Mogwai.ConstitutionMod);
            Assert.Equal(1, Mogwai.InteligenceMod);
            Assert.Equal(2, Mogwai.WisdomMod);
            Assert.Equal(4, Mogwai.CharismaMod);

            Mogwai.EquipeItem(SlotType.Ring, ringOfTheBear, 0);

            Assert.Equal(1, Mogwai.Inventory.Count);
            Assert.Equal(1, Mogwai.StrengthMod);
            Assert.Equal(0, Mogwai.DexterityMod);
            Assert.Equal(0, Mogwai.ConstitutionMod);
            Assert.Equal(0, Mogwai.InteligenceMod);
            Assert.Equal(1, Mogwai.WisdomMod);
            Assert.Equal(3, Mogwai.CharismaMod);
        }
    }
}