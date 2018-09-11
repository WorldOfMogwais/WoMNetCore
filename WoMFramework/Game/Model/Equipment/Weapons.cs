using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public class Weapons
    {
        /***
         * Unarmed Weapons
         */
        public static Weapon Gauntlet =>
            WeaponBuilder.Create("Gauntlet", new int[] { 1, 2 }, new int[] { 1, 3 })
            .SetDamageType(WeaponDamageType.BLUDGEONING)
            .SetRange(1)
            .SetCost(2)
            .SetWeight(1)
            .SetDescription(" This metal glove lets you deal lethal damage rather than nonlethal " +
                "damage with unarmed strikes. A strike with a gauntlet is otherwise considered an " +
                "unarmed attack. Your opponent cannot use a disarm action to disarm you of " +
                "gauntlets.")
            .Build();


        /***
         * One-Handed Melee Weapons
         */
        public static Weapon Rapier =>
            WeaponBuilder.Create("Rapier", new int[] { 1, 4 }, new int[] { 1, 6 })
            .SetCriticalMinRoll(18)
            .SetDamageType(WeaponDamageType.PIERCING)
            .SetRange(1)
            .SetCost(20)
            .SetWeight(2)
            .SetDescription("Just a rapier.")
            .Build();


        /***
         * Two-Handed Melee Weapons
         */
        public static Weapon Spear =>
            WeaponBuilder.Create("Spear", new int[] { 1, 6 }, new int[] { 1, 8 })
            .SetCriticalMultiplier(3)
            .SetDamageType(WeaponDamageType.PIERCING)
            .IsTwoHanded()
            .SetRange(20)
            .SetCost(2)
            .SetWeight(6)
            .SetDescription("A spear is 5 feet in length and can be thrown.")
            .Build();


    }
}
