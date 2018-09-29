using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public class Weapons
    {
        /***
         * One-Handed Melee Weapons
         */
        public static Weapon Rapier =>
            WeaponBuilder.Create("Rapier", WeaponProficiencyType.Martial, WeaponEffortType.OneHanded, new[] { 1, 4 }, new[] { 1, 6 })
            .SetCriticalMinRoll(18)
            .SetCriticalMultiplier(2)
            .SetDamageType(WeaponDamageType.Piercing)
            .SetRange(1)
            .SetCost(20)
            .SetWeight(2)
            .SetDescription("Just a rapier.")
            .Build();
    }
}
