namespace WoMFrameworkTest.Game
{
    using WoMFramework.Game.Model.Monster;
    using Xunit;

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
    }
}
