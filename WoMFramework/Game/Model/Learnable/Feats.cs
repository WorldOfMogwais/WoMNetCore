namespace WoMFramework.Game.Model.Learnable
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Tool;

    public class Feats
    {
        private const string DefaultFeatFile = "Feats.json";

        private static Feats _instance;

        private readonly List<Feat> _featsFile;

        public Feats(string path = DefaultFeatFile)
        {
            // load feats file
            if (!Caching.TryReadFile(path, out _featsFile))
            {
                throw new Exception($"Couldn't find {path} database file.");
            }
        }

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
