namespace WoMFramework.Game.Model.Monster
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Tool;

    public class Monsters
    {
        private const string DefaultMonsterFile = "MonsterBuilders.json";

        private static Monsters _instance;

        private readonly List<MonsterBuilder> _monsterBuilders;

        private Monsters(string path = DefaultMonsterFile)
        {
            // load monsters file
            if (!Caching.TryReadFile(path, out _monsterBuilders))
            {
                throw new Exception($"Couldn't find {path} database file.");
            }

            // only  for testing purpose
            //var _monstersFile = monsterBuilders.Select(p => p.Build()).ToList();
        }

        public static Monsters Instance => _instance ?? (_instance = new Monsters());

        public Monster ByName(string monsterName)
        {
            var monsterBuilder = _monsterBuilders.FirstOrDefault(p => p.Name == monsterName);
            if (monsterBuilder == null)
            {
                throw new Exception($"Unknown monster please check database '{monsterName}'");
            }

            return monsterBuilder.Build();
        }

        public List<MonsterBuilder> AllBuilders()
        {
            return _monsterBuilders;
        }
    }
}
