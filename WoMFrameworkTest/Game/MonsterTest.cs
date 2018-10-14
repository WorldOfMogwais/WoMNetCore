using WoMFramework.Game.Model.Monster;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class MonsterTest
    {
        [Fact]
        public void MonsterCombatActionTest()
        {
            var rat = Monsters.Rat;
            Assert.True(rat.CombatActions.Count > 0);
            Assert.Equal("Move", rat.CombatActions[0].GetType().Name);
            Assert.Equal("UnarmedAttack", rat.CombatActions[1].GetType().Name);
        }
    }
}
