using System.Collections.Generic;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public class Reward
    {
        public int Exp { get; }

        public int Gold { get; }

        public List<BaseItem> Items { get; }

        public Reward(int exp, int gold, List<BaseItem> items)
        {
            Exp = exp;
            Gold = gold;
            Items = items;
        }
    }
}