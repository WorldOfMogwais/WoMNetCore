namespace WoMFrameworkTest.Game
{
    using WoMFramework.Game.Enums;
    using WoMFramework.Game.Model;
    using WoMFramework.Game.Model.Monster;
    using Xunit;

    [Collection("SystemInteractionFixture")]
    public class WeaponTest
    {
        [Fact]
        public void WeaponTestSizeType()
        {
            Assert.Equal(1, Monsters.Instance.ByName("Rat").Equipment.WeaponSlots[0].PrimaryWeapon.DamageRoll[0]);
            Assert.Equal(3, Monsters.Instance.ByName("Rat").Equipment.WeaponSlots[0].PrimaryWeapon.DamageRoll[1]);
            Weapon rapier = Weapons.Instance.ByName("Rapier");
            Assert.Equal(1, rapier.DamageRoll[0]);
            Assert.Equal(6, rapier.DamageRoll[1]);
            rapier.SetSize(SizeType.Tiny);
            Assert.Equal(1, rapier.DamageRoll[0]);
            Assert.Equal(3, rapier.DamageRoll[1]);
            rapier.SetSize(SizeType.Huge);
            Assert.Equal(2, rapier.DamageRoll[0]);
            Assert.Equal(6, rapier.DamageRoll[1]);
        }
    }
}
