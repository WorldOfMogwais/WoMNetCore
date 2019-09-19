namespace WoMFramework.Game.Generator
{
    using Enums;
    using Model;
    using Random;
    using System.Collections.Generic;

    public class Treasure
    {
        public int Gold { get; }

        public List<BaseItem> Items { get; }

        public Treasure()
        {
            Gold = 0;
            Items = new List<BaseItem>();
        }

        public Treasure(int gold)
        {
            this.Gold = gold;
            Items = new List<BaseItem>();
        }

        public Treasure(int gold, List<BaseItem> items)
        {
            this.Gold = gold;
            this.Items = items;
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
