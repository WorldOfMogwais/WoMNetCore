using Xunit;

namespace WoMFramework.Game.Model.Tests
{
    public class ClassesTest
    {
        [Fact]
        public void NoClasstest()
        {
            var noClass = new NoClass();
            Assert.Single(noClass.BaseAttackBonus);
            Assert.Equal(0, noClass.BaseAttackBonus[0]);
            Assert.Equal(0, noClass.ClassLevel);
            noClass.ClassLevelUp();
            Assert.Equal(1, noClass.ClassLevel);
        }

        [Fact]
        public void BarbarianTest()
        {

            var barbarian = new Barbarian();

            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(0, barbarian.BaseAttackBonus[0]);
            Assert.Equal(0, barbarian.ClassLevel);
            Assert.Equal(0, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(1, barbarian.BaseAttackBonus[0]);
            Assert.Equal(1, barbarian.ClassLevel);
            Assert.Equal(2, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(2, barbarian.BaseAttackBonus[0]);
            Assert.Equal(2, barbarian.ClassLevel);
            Assert.Equal(3, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(3, barbarian.BaseAttackBonus[0]);
            Assert.Equal(3, barbarian.ClassLevel);
            Assert.Equal(3, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(4, barbarian.BaseAttackBonus[0]);
            Assert.Equal(4, barbarian.ClassLevel);
            Assert.Equal(4, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Single(barbarian.BaseAttackBonus);
            Assert.Equal(5, barbarian.BaseAttackBonus[0]);
            Assert.Equal(5, barbarian.ClassLevel);
            Assert.Equal(4, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Equal(2, barbarian.BaseAttackBonus.Length);
            Assert.Equal(6, barbarian.BaseAttackBonus[0]);
            Assert.Equal(1, barbarian.BaseAttackBonus[1]);
            Assert.Equal(6, barbarian.ClassLevel);
            Assert.Equal(5, barbarian.FortitudeBaseSave);
            Assert.Equal(2, barbarian.ReflexBaseSave);
            Assert.Equal(2, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Equal(2, barbarian.BaseAttackBonus.Length);
            Assert.Equal(7, barbarian.BaseAttackBonus[0]);
            Assert.Equal(2, barbarian.BaseAttackBonus[1]);
            Assert.Equal(7, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(2, barbarian.BaseAttackBonus.Length);
            Assert.Equal(8, barbarian.BaseAttackBonus[0]);
            Assert.Equal(3, barbarian.BaseAttackBonus[1]);
            Assert.Equal(8, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(2, barbarian.BaseAttackBonus.Length);
            Assert.Equal(9, barbarian.BaseAttackBonus[0]);
            Assert.Equal(4, barbarian.BaseAttackBonus[1]);
            Assert.Equal(9, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(2, barbarian.BaseAttackBonus.Length);
            Assert.Equal(10, barbarian.BaseAttackBonus[0]);
            Assert.Equal(5, barbarian.BaseAttackBonus[1]);
            Assert.Equal(10, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(3, barbarian.BaseAttackBonus.Length);
            Assert.Equal(11, barbarian.BaseAttackBonus[0]);
            Assert.Equal(6, barbarian.BaseAttackBonus[1]);
            Assert.Equal(1, barbarian.BaseAttackBonus[2]);
            Assert.Equal(11, barbarian.ClassLevel);
        }
    }
}
