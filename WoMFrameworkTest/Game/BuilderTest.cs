namespace WoMFrameworkTest.Game
{
    using System.Linq;
    using WoMFramework.Game.Enums;
    using WoMFramework.Game.Model;
    using WoMFramework.Game.Model.Monster;
    using Xunit;

    [Collection("SystemInteractionFixture")]
    public class BuilderTest
    {
        [Fact]
        public void BuilderCreationTest()
        {
            var allMonster = Monsters.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(2, allMonster.Count());
            var allArmor = Armors.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(62, allArmor.Count());
            var allWeapons = Weapons.Instance.AllBuilders() .Select(p => p.Build());
            Assert.Equal(212, allWeapons.Count());
        }

        [Fact]
        public void BuilderMonsterTest()
        {
            var allMonster = Monsters.Instance.AllBuilders().Select(p => p.Build());
            Assert.Equal(1, allMonster.Where(p => (p.EnvironmentTypes.Contains(EnvironmentType.Any)
                                                 || p.EnvironmentTypes.Contains(EnvironmentType.Undergrounds))
                                                && p.ChallengeRating <= 0.5).Count());
            Assert.Equal(1, allMonster.Where(p => p.ChallengeRating < 1).Count());
            Assert.Equal( 1, allMonster.Where(p => p.ChallengeRating == 1).Count());
            Assert.Equal(0, allMonster.Where(p => p.ChallengeRating == 2).Count());
            Assert.Equal(0, allMonster.Where(p => p.ChallengeRating == 3).Count());
            Assert.Equal("Wolf", allMonster.Where(p => p.ChallengeRating == 1 && p.NaturalArmor == 2).First().Name);
        }
    }
}
