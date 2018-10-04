using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public class Weapons
    {
        /***
         *
         */
        public static Weapon UnarmedStrike =>
            WeaponBuilder.Create("Unarmed Strike", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 3 })
                .SetCriticalMinRoll(20)
                .SetCriticalMultiplier(2)
                .SetDamageType(WeaponDamageType.Bludgeoning)
                .SetRange(1)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("An unarmed strike is the default attack a character makes when not equipped with weapons.")
                .Build();
        /***
         * One-Handed Melee Weapons
         */
        public static Weapon Rapier =>
            WeaponBuilder.Create("Rapier", WeaponProficiencyType.Martial, WeaponEffortType.OneHanded, new[] { 1, 6 })
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
