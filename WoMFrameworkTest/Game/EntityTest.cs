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
        private string _mogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";

        private string _pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode("MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i"));

        private int _blockHeight = 2000;
        private int _index = 0;

        public Mogwai InitMogwai(List<Shift> addShifts)
        {

            var shifts = new Dictionary<double, Shift>
            {
                {
                    _blockHeight, new Shift(_index++, 1530914381, _pubMogAddressHex,
                        _blockHeight, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                        2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                        1.00m,
                        0.0001m)
                }
            };

            foreach (var shft in addShifts)
            {
                _blockHeight++;
                _index++;
                shifts.Add(_blockHeight, new Shift(_index, shft.Time, shft.AdHex, _blockHeight, shft.BkHex, shft.BkIndex, shft.TxHex, shft.Amount, shft.Fee));
            }

            return new Mogwai(_mogAddress, shifts);
        }

        [Fact]
        public void BasicEntity()
        {
            var mogwai = InitMogwai(new List<Shift>());
            while (mogwai.Evolve(out _))
            {
            }
            Assert.Equal("Heckaece", mogwai.Name);

        }

        [Fact]
        public void RequirementTest()
        {
            var lvlBarbarian = new LevelingAction(LevelingType.Class, ClassType.Barbarian, 0, 1);
            var mogwai = InitMogwai(new List<Shift>()
            {
                new Shift(0, 1530914381, _pubMogAddressHex,
                    0, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                    2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                    lvlBarbarian.GetValue1(),
                    lvlBarbarian.GetValue2())
            });
            while (mogwai.Evolve(out _))
            {
            }
            Assert.Equal(1, mogwai.GetRequirementValue(RequirementType.FighterLevel, 1));
            Assert.Equal(0, mogwai.GetRequirementValue(RequirementType.CasterLevel, 0));

            var lvlSorcerer = new LevelingAction(LevelingType.Class, ClassType.Sorcerer, 0, 1);
            mogwai = InitMogwai(new List<Shift>()
            {
                new Shift(0, 1530914381, _pubMogAddressHex,
                    0, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                    2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                    lvlSorcerer.GetValue1(),
                    lvlSorcerer.GetValue2())
            });
            while (mogwai.Evolve(out _))
            {
            }
            Assert.Equal(0, mogwai.GetRequirementValue(RequirementType.FighterLevel));
            Assert.Equal(1, mogwai.GetRequirementValue(RequirementType.CasterLevel));

        }
    }
}
