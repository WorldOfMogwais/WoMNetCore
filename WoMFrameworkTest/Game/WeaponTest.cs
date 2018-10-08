using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Equipment;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Tool;
using Xunit;

namespace WoMFrameworkTest.Game
{
    public class WeaponTest
    {
        [Fact]
        public void WeaponTestSizeType()
        {
            Assert.Equal(1, Monsters.Rat.Equipment.WeaponSlots[0].PrimaryWeapon.DamageRoll[0]);
            Assert.Equal(3, Monsters.Rat.Equipment.WeaponSlots[0].PrimaryWeapon.DamageRoll[1]);
            var rapier = Weapons.Rapier;
            Assert.Equal(1, rapier.DamageRoll[0]);
            Assert.Equal(6, rapier.DamageRoll[1]);
            rapier.WeaponSizeType = SizeType.Tiny;
            Assert.Equal(1, rapier.DamageRoll[0]);
            Assert.Equal(3, rapier.DamageRoll[1]);
            rapier.WeaponSizeType = SizeType.Huge;
            Assert.Equal(2, rapier.DamageRoll[0]);
            Assert.Equal(6, rapier.DamageRoll[1]);
        }
    }
}
