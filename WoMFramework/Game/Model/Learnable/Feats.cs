using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class Feats
    {
        private const string DefaultFeatFile = "feats.json";

        private static Feats _instance;

        private readonly List<Feat> _featsFile;

        private Feats(string path = DefaultFeatFile)
        {
            // load feats file
            if (!Caching.TryReadFile(path, out _featsFile))
            {
                throw new Exception("couldn't find the feats database file.");
            }
        }

        public static void InstanceWithPath(string path) => _instance = new Feats(path);

        public static Feats Instance => _instance ?? (_instance = new Feats());

        public Feat ByName(string featName)
        {
            return _featsFile.FirstOrDefault(p => p.Name == featName);
        }

        public Feat ById(int featId)
        {
            return _featsFile.FirstOrDefault(p => p.Id == featId);
        }
    }

}
