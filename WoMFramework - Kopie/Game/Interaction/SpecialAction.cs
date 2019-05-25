using System;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Interaction
{
    public enum SpecialType
    {
        None = 0,
        Heal = 1,
        Reviving = 2
    }

    public enum SpecialSubType
    {
        None = 0
    }

    public class SpecialAction : Interaction
    {
        public SpecialType SpecialType { get; }

        public SpecialSubType SpecialSubType { get; }

        public SpecialAction(SpecialType specialType, SpecialSubType specialSubType, CostType costType) : base(InteractionType.Special, costType)
        {
            SpecialType = specialType;
            SpecialSubType = specialSubType;
            ParamAdd1 = (int)specialType * 100 + (int)specialSubType;
            // not really used, but can be freed later ...
            ParamAdd2 = 0;
        }

        public override string GetInfo()
        {
            return InteractionType + ", "
                 + SpecialType + ", "
                 + SpecialSubType;
        }

        public static bool TryGetInteraction(int paramAdd1, int paramAdd2, out SpecialAction special)
        {
            if (Enum.IsDefined(typeof(SpecialType), int.Parse(paramAdd1.ToString("0000").Substring(0, 2)))
             && Enum.IsDefined(typeof(SpecialSubType), int.Parse(paramAdd1.ToString("0000").Substring(2, 2))))
            {
                special = new SpecialAction(paramAdd1, paramAdd2);
                return true;
            }

            special = null;
            return false;
        }

        private SpecialAction(int paramAdd1, int paramAdd2) : base(InteractionType.Special)
        {
            var param1 = paramAdd1.ToString("0000");
            SpecialType = (SpecialType)int.Parse(param1.Substring(0, 2));
            SpecialSubType = (SpecialSubType)int.Parse(param1.Substring(2, 2));

            var param2 = paramAdd2.ToString("0000");
            //notused1 = int.Parse(param2.Substring(0, 2));
            //notused2 = int.Parse(param2.Substring(2, 2));

            ParamAdd1 = paramAdd1;
            ParamAdd2 = paramAdd2;
        }

    }
}
