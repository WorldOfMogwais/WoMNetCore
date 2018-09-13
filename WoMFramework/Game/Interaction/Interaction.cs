using System;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Interaction
{

    public abstract class Interaction
    {
        public CostType CostType { get; set; }

        public InteractionType InteractionType { get; set; }

        public int ParamAdd1 { get; set; }

        public int ParamAdd2 { get; set; }

        public Interaction(InteractionType interactionType)
        {
            CostType = CostType.Standard;
            InteractionType = interactionType;
        }

        public decimal GetValue1()
        {
            var value = (int)CostType * 1000000 + (int)InteractionType * 10000 + ParamAdd1;
            return decimal.Parse("0." + value.ToString().PadLeft(8, '0'));
        }

        public decimal GetValue2()
        {
            return decimal.Parse("0." + ParamAdd2.ToString().PadLeft(8, '0'));
        }

        public static Interaction GetInteraction(decimal amount, decimal fee)
        {
            var parm1 = amount.ToString("0.00000000").Split('.')[1];
            var costTypeInt = int.Parse(parm1.Substring(0, 2));
            var interactionTypInt = int.Parse(parm1.Substring(2, 2));
            if (Enum.IsDefined(typeof(CostType), costTypeInt)
             && Enum.IsDefined(typeof(InteractionType), interactionTypInt))
            {
                var costType = (CostType)costTypeInt;
                var interactionType = (InteractionType)interactionTypInt;
                var addParam1 = int.Parse(parm1.Substring(4, 4));
                var addParam2 = int.Parse(fee.ToString("0.00000000").Split('.')[1].Substring(4, 4));

                switch (interactionType)
                {
                    case InteractionType.Adventure:
                        if (AdventureAction.TryGetAdventure(addParam1, addParam2, out var adventure))
                        {
                            return adventure;
                        }
                        break;
                    case InteractionType.Leveling:
                        if (LevelingAction.TryGetAdventure(addParam1, addParam2, out var leveling))
                        {
                            return leveling;
                        }
                        break;
                }
            }

            return new Unknown();
        }

    }
}
