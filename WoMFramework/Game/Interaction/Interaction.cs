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

        protected Interaction(InteractionType interactionType)
        {
            CostType = CostType.Standard;
            InteractionType = interactionType;
        }

        protected Interaction(InteractionType interactionType, CostType costType)
        {
            CostType = costType;
            InteractionType = interactionType;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public decimal Cost()
        {
            return GetValue1() + GetValue2();
        }

        public virtual string GetInfo()
        {
            return InteractionType.ToString();
        }

        public decimal GetValue1()
        {
            var value = (int)CostType * 1000000 + (int)InteractionType * 10000 + ParamAdd1;
            //return decimal.Parse("0." + value.ToString().PadLeft(8, '0'));
            return (decimal)value / 100000000;
        }

        public decimal GetValue2()
        {
            //return decimal.Parse("0." + ParamAdd2.ToString().PadLeft(8, '0'));
            return (decimal)ParamAdd2 / 100000000;
        }

        public static Interaction GetInteraction(decimal amount, decimal fee)
        {

            var parm1 = ((int)(amount * 100000000 % 100000000)).ToString("00000000");
            //var parm1 = amount.ToString("0.00000000").Split('.')[1];
            var costTypeInt = int.Parse(parm1.Substring(0, 2));
            //var costTypeInt = (int) (amount * 100 % 100);
            var interactionTypInt = int.Parse(parm1.Substring(2, 2));
            //var interactionTypInt = (int) (amount * 10000 % 10000) % (costTypeInt * 100);

            if (Enum.IsDefined(typeof(CostType), costTypeInt)
             && Enum.IsDefined(typeof(InteractionType), interactionTypInt))
            {
                var interactionType = (InteractionType)interactionTypInt;
                var addParam1 = int.Parse(parm1.Substring(4, 4));

                var feeString = ((int)(fee * 100000000 % 100000000)).ToString("00000000");
                var addParam2 = int.Parse(feeString.Substring(4, 4));
                //var addParam2 = int.Parse(fee.ToString("0.00000000").Split('.')[1].Substring(4, 4));

                switch (interactionType)
                {
                    case InteractionType.Adventure:
                        if (AdventureAction.TryGetInteraction(addParam1, addParam2, out var adventure))
                        {
                            return adventure;
                        }
                        break;

                    case InteractionType.Leveling:
                        if (LevelingAction.TryGetInteraction(addParam1, addParam2, out var leveling))
                        {
                            return leveling;
                        }
                        break;

                    case InteractionType.Special:
                        if (SpecialAction.TryGetInteraction(addParam1, addParam2, out var special))
                        {
                            return special;
                        }
                        break;
                }
            }

            return new Unknown();
        }

    }
}
