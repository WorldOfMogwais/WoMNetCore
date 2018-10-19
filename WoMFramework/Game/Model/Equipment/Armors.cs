using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class Armors
    {
        private const string DefaultArmorFile = "Armors.json";

        private static Armors _instance;

        private readonly List<Armor> _armorsFile;

        private Armors(string path = DefaultArmorFile)
        {
            // load weapons file
            if (!Caching.TryReadFile(path, out _armorsFile))
            {
                throw new Exception("couldn't find the armors database file.");
            }
        }

        public static Armors Instance => _instance ?? (_instance = new Armors());

        public Armor ByName(string armorName)
        {
            return _armorsFile.FirstOrDefault(p => p.Name == armorName);
        } 

    }
}
