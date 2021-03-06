﻿namespace WoMFramework.Game.Interaction
{
    using Enums;
    using System;

    public class LevelingAction : Interaction
    {
        public LevelingType LevelingType { get; }

        public ClassType ClassType { get; }

        public int CurrentLevel { get; }

        public int ClassLevel { get; }

        public LevelingAction(LevelingType levelingType, ClassType classType, int currentLevel, int classLevel) : base(InteractionType.Leveling)
        {
            LevelingType = levelingType;
            ClassType = classType;
            CurrentLevel = currentLevel;
            ClassLevel = classLevel;
            ParamAdd1 = (int)levelingType * 100 + (int)classType;
            ParamAdd2 = currentLevel * 100 + classLevel;
        }

        public override string GetInfo()
        {
            return InteractionType + ", "
                 + LevelingType + ", "
                 + ClassType + ", "
                 + CurrentLevel + ", "
                 + ClassLevel;
        }

        public static bool TryGetInteraction(int paramAdd1, int paramAdd2, out LevelingAction leveling)
        {
            if (Enum.IsDefined(typeof(LevelingType), int.Parse(paramAdd1.ToString("0000").Substring(0, 2)))
             && Enum.IsDefined(typeof(ClassType), int.Parse(paramAdd1.ToString("0000").Substring(2, 2))))
            {
                leveling = new LevelingAction(paramAdd1, paramAdd2);
                return true;
            }

            leveling = null;
            return false;
        }

        private LevelingAction(int paramAdd1, int paramAdd2) : base(InteractionType.Leveling)
        {
            var param1 = paramAdd1.ToString("0000");
            LevelingType = (LevelingType)int.Parse(param1.Substring(0, 2));
            ClassType = (ClassType)int.Parse(param1.Substring(2, 2));

            var param2 = paramAdd2.ToString("0000");
            CurrentLevel = int.Parse(param2.Substring(0, 2));
            ClassLevel = int.Parse(param2.Substring(2, 2));

            ParamAdd1 = paramAdd1;
            ParamAdd2 = paramAdd2;
        }
    }
}
