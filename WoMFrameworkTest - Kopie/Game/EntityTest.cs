using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class EntityTest
    {
        public const string MogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";

        public Mogwai Mogwai1;

        public Mogwai Mogwai2;

        public EntityTest()
        {
            string pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(MogAddress));

            var lvlClass1 = new LevelingAction(LevelingType.Class, ClassType.Barbarian, 0, 1);
            var creation1 = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            Assert.True(creation1.History != null);

            var level1 = new Shift(1, 1530914381, pubMogAddressHex,
                2001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                lvlClass1.GetValue1(),
                lvlClass1.GetValue2());
            Assert.True(level1.History != null);

            var shifts1 = new Dictionary<long, Shift>
            {
                {2000, creation1},
                {2001, level1}
            };

            Mogwai1 = new Mogwai(MogAddress, shifts1);

            var lvlClass2 = new LevelingAction(LevelingType.Class, ClassType.Sorcerer, 0, 1);
            var creation2 = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            Assert.True(creation2.History != null);

            var level2 = new Shift(1, 1530914381, pubMogAddressHex,
                2001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                lvlClass2.GetValue1(),
                lvlClass2.GetValue2());
            Assert.True(level2.History != null);

            var shifts2 = new Dictionary<long, Shift>
            {
                {2000, creation2},
                {2001, level2}
            };
            Mogwai2 = new Mogwai(MogAddress, shifts2);

        }


        [Fact]
        public void BasicEntity()
        {

            Assert.Equal("Heckaece", Mogwai1.Name);
        }

        [Fact]
        public void RequirementFighterTest()
        {

            Mogwai1.Evolve();

            Assert.Equal(1, Mogwai1.GetRequirementValue(RequirementType.FighterLevel, 1));
            Assert.Equal(0, Mogwai1.GetRequirementValue(RequirementType.CasterLevel, 0));
        }

        [Fact]
        public void RequirementCasterTest()
        {

            Mogwai2.Evolve();

            Assert.Equal(0, Mogwai2.GetRequirementValue(RequirementType.FighterLevel));
            Assert.Equal(1, Mogwai2.GetRequirementValue(RequirementType.CasterLevel));
        }
    }
}
