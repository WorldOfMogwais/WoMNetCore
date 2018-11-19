using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class SpellTest : IDisposable
    {
        public const string MogAddress = "MJHYMxu2kyR1ci4pYwktbeCM7yjZyVxt2i";

        public Mogwai Mogwai;

        public SpellTest()
        {
            var lvlCleric = new LevelingAction(LevelingType.Class, ClassType.Cleric, 0, 1);

            var pubMogAddressHex = HexHashUtil.ByteArrayToString(Base58Encoding.Decode(MogAddress));

            var creation = new Shift(0, 1530914381, pubMogAddressHex,
                2000, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                1.00m,
                0.0001m);
            Assert.True(creation.History != null);

            var level = new Shift(1, 1530914381, pubMogAddressHex,
                2001, "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
                2, "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
                lvlCleric.GetValue1(),
                lvlCleric.GetValue2());
            Assert.True(level.History != null);

            var shifts = new Dictionary<long, Shift>
            {
                {2000, creation},
                {2001, level}
            };
            Mogwai = new Mogwai(MogAddress, shifts);
        }

        [Fact]
        public void BasicSpellTest()
        {           
            if (Mogwai.Evolve())
            {
                Assert.Single(Mogwai.Classes.First(p => p.ClassType == ClassType.Cleric).Learnables);
                //Assert.True(mogwai.Learn(Spells.CureLightWounds()));
                Assert.True(Mogwai.CombatActions.Exists(p => p is SpellCast));
            }
            else
            {
                Assert.True(false);
            }
        }

        public void Dispose()
        {
            Mogwai = null;
        }
    }
}
