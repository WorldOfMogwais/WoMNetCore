namespace WoMFrameworkTest.Game
{
    using WoMFramework.Game.Model.Monster;
    using Xunit;
    using System.Linq;
    using System.Diagnostics;

    [Collection("SystemInteractionFixture")]
    public class MonsterTest
    {
        [Fact]
        public void MonsterCombatActionTest()
        {
            Monster rat = Monsters.Instance.ByName("Rat");
            Assert.True(rat.CombatActions.Count > 0);
            Assert.Equal("Move", rat.CombatActions[0].GetType().Name);
            Assert.Equal("UnarmedAttack", rat.CombatActions[1].GetType().Name);
        }

        [Fact]
        public void MonsterCountTest()
        {
            var monsterSubEqual1 = Monsters.Instance.AllBuilders().Where(p => p.ChallengeRating <= 1).ToList();
            Assert.Equal(184, monsterSubEqual1.Count());
            Monster bear = Monsters.Instance.ByName("Polar Bear");
            Debug.Print($"Name: {bear.Name}");
        }
    }
}
