using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Monster;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class BuilderTest
    {
        [Fact]
        public void BuilderCreationTest()
        {
            var allMonster = Monsters.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(1183, allMonster.Count());
            var allArmor = Armors.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(62, allArmor.Count());
            var allWeapons = Weapons.Instance.AllBuilders() .Select(p => p.Build());
            Assert.Equal(212, allWeapons.Count());
        }

        [Fact]
        public void BuilderMonsterTest()
        {
            var allMonster = Monsters.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(37, allMonster.Where(p => (p.EnvironmentTypes.Contains(EnvironmentType.Any)
                                                 || p.EnvironmentTypes.Contains(EnvironmentType.Undergrounds))
                                                && p.ChallengeRating <= 0.5).Count());
            Assert.Equal(101, allMonster.Where(p => p.ChallengeRating < 1).Count());
            Assert.Equal( 83, allMonster.Where(p => p.ChallengeRating == 1).Count());
            Assert.Equal(121, allMonster.Where(p => p.ChallengeRating == 2).Count());
            Assert.Equal(114, allMonster.Where(p => p.ChallengeRating == 3).Count());
            Assert.Equal("Chickcharney", allMonster.Where(p => p.ChallengeRating == 3 && p.NaturalArmor == 0).First().Name);

        }
    }
}
