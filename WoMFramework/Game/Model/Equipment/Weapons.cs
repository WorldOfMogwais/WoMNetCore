namespace WoMFramework.Game.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Tool;

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
                throw new Exception($"Couldn't find {path} database file.");
            }
        }

        public static Weapons Instance => _instance ?? (_instance = new Weapons(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location), DefaultWeaponFile)));

        public Weapon ByName(string weaponName)
        {
            var weaponBuilder = _weaponBuilders.FirstOrDefault(p => p.Name == weaponName);
            if (weaponBuilder == null)
            {
                throw new Exception($"Unknown weapon please check database '{weaponName}'");
            }

            return weaponBuilder.Build();
        }

        public List<WeaponBuilder> AllBuilders()
        {
            return _weaponBuilders;
        }
    }
}
