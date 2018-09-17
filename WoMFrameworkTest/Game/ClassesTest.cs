using Xunit;

namespace WoMFramework.Game.Model.Tests
{
    public class ClassesTest
    {
        [Fact]
        public void NoClasstest()
        {
            var noClass = new NoClass();
            Assert.Equal(0, noClass.ClassAttackBonus);
            Assert.Equal(0, noClass.ClassLevel);
            noClass.ClassLevelUp();
            Assert.Equal(1, noClass.ClassLevel);
        }

        [Fact]
        public void BarbarianTest()
        {

            var barbarian = new Barbarian();

            Assert.Equal(0, barbarian.ClassAttackBonus);
            Assert.Equal(0, barbarian.ClassLevel);
            Assert.Equal(0, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Equal(1, barbarian.ClassAttackBonus);
            Assert.Equal(1, barbarian.ClassLevel);
            Assert.Equal(2, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            
            Assert.Equal(2, barbarian.ClassAttackBonus);
            Assert.Equal(2, barbarian.ClassLevel);
            Assert.Equal(3, barbarian.FortitudeBaseSave);
            Assert.Equal(0, barbarian.ReflexBaseSave);
            Assert.Equal(0, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            
            Assert.Equal(3, barbarian.ClassAttackBonus);
            Assert.Equal(3, barbarian.ClassLevel);
            Assert.Equal(3, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            
            Assert.Equal(4, barbarian.ClassAttackBonus);
            Assert.Equal(4, barbarian.ClassLevel);
            Assert.Equal(4, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            
            Assert.Equal(5, barbarian.ClassAttackBonus);
            Assert.Equal(5, barbarian.ClassLevel);
            Assert.Equal(4, barbarian.FortitudeBaseSave);
            Assert.Equal(1, barbarian.ReflexBaseSave);
            Assert.Equal(1, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Equal(6, barbarian.ClassAttackBonus);
            Assert.Equal(6, barbarian.ClassLevel);
            Assert.Equal(5, barbarian.FortitudeBaseSave);
            Assert.Equal(2, barbarian.ReflexBaseSave);
            Assert.Equal(2, barbarian.WillBaseSave);
            barbarian.ClassLevelUp();
            Assert.Equal(7, barbarian.ClassAttackBonus);
            Assert.Equal(7, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(8, barbarian.ClassAttackBonus);
            Assert.Equal(8, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(9, barbarian.ClassAttackBonus);
            Assert.Equal(9, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(10, barbarian.ClassAttackBonus);
            Assert.Equal(10, barbarian.ClassLevel);
            barbarian.ClassLevelUp();
            Assert.Equal(11, barbarian.ClassAttackBonus);
            Assert.Equal(11, barbarian.ClassLevel);
        }
    }
}
