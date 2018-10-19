using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class Weapons
    {
        private const string DefaultWeaponFile = "weapons.json";

        private static Weapons _instance;

        private readonly List<Weapon> _weaponsFile;

        private Weapons(string path = DefaultWeaponFile)
        {
            // load weapons file
            if (!Caching.TryReadFile(path, out _weaponsFile))
            {
                throw new Exception("couldn't find the weapons database file.");
            }
        }

        public static Weapons Instance => _instance ?? (_instance = new Weapons());

        public Weapon ByName(string weaponName)
        {
            return _weaponsFile.FirstOrDefault(p => p.Name == weaponName);
        } 

        /***
         *
         */
        public static Weapon UnarmedStrike =>
            WeaponBuilder.Create("Unarmed Strike", WeaponBaseType.UnarmedStrike, WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 3 })
                .SetCriticalMinRoll(20)
                .SetCriticalMultiplier(2)
                .SetDamageType(WeaponDamageType.Bludgeoning)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("An unarmed strike is the default attack a character makes when not equipped with weapons.")
                .Build();
        /***
         * One-Handed Melee Weapons
         */
        public static Weapon Rapier =>
            WeaponBuilder.Create("Rapier",  WeaponBaseType.Rapier, WeaponProficiencyType.Martial, WeaponEffortType.OneHanded, new[] { 1, 6 })
            .SetCriticalMinRoll(18)
            .SetCriticalMultiplier(2)
            .SetDamageType(WeaponDamageType.Piercing)
            .SetCost(20)
            .SetWeight(2)
            .SetDescription("Just a rapier.")
            .Build();
    }
}
