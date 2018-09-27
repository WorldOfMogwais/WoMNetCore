using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public class Weapons
    {
        /***
         * One-Handed Melee Weapons
         */
        public static Weapon Rapier =>
            WeaponBuilder.Create("Rapier", ProficiencyType.Martial, WeaponType.OneHanded, new[] { 1, 4 }, new[] { 1, 6 })
            .SetCriticalMinRoll(18)
            .SetCriticalMultiplier(2)
            .SetDamageType(WeaponDamageType.Piercing)
            .SetRange(0)
            .SetCost(20)
            .SetWeight(2)
            .SetDescription("Just a rapier.")
            .Build();
    }
}
