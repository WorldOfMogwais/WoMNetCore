using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class Weapons
    {
        private const string DefaultWeaponFile = "WeaponBuilders.json";

        private static Weapons _instance;

        private readonly List<WeaponBuilder> _weaponBuilders;

        private Weapons(string path = DefaultWeaponFile)
        {
            // load weapons file
            if (!Caching.TryReadFile(path, out _weaponBuilders))
            {
                throw new Exception("couldn't find the weaponbuilders database file.");
            }
        }

        public static Weapons Instance => _instance ?? (_instance = new Weapons());

        public Weapon ByName(string weaponName)
        {
            var weaponBuilder = _weaponBuilders.FirstOrDefault(p => p.Name == weaponName);
            if (weaponBuilder == null)
            {
                throw new Exception($"Unknown weapon please check database '{weaponName}'");
            }
            return weaponBuilder.Build();
        }

        public List<WeaponBuilder> AllWeaponBuilder()
        {
            return _weaponBuilders;
        }
    }
}
