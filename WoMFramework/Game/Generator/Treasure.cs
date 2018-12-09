using System.Collections.Generic;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public class Treasure
    {
        public int Gold { get; }

        public List<BaseItem> Items { get; }

        public Treasure()
        {
            Items = new List<BaseItem>();
        }
    }
}