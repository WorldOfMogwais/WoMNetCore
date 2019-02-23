using System;
using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Generator
{
    public class Treasure
    {
        public int Gold { get; }

        public List<BaseItem> Items { get; }

        public Treasure()
        {
            this.Gold = 0;
            this.Items = new List<BaseItem>();
        }

        public Treasure(int Gold)
        {
            this.Gold = Gold;
            this.Items = new List<BaseItem>();
        }

        public Treasure(int Gold, List<BaseItem> Items)
        {
            this.Gold = Gold;
            this.Items = Items;
        }

        public static Treasure Create(Dice dice, TreasureType treasureType)
        {
            int roll = dice.Roll(DiceType.D100);

            switch (treasureType)
            {
                case TreasureType.Incidental:
                    return roll < 11 ? new Treasure(1) : new Treasure(0);

                case TreasureType.Standard:
                    return new Treasure(1);

                case TreasureType.Double:
                    return new Treasure(2);

                case TreasureType.Triple:
                    return new Treasure(3);

                case TreasureType.None:
                    return roll == 1 ? new Treasure(1) : new Treasure(0);

                default:
                    return new Treasure(0);
            }
        }
    }
}