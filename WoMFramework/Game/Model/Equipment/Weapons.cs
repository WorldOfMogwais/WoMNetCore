using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class Weapons
    {
        private const string DefaultWeaponFile = "Weapons.json";

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

        public List<Weapon> All()
        {
            return _weaponsFile;
        }
    }
}
