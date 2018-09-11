using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Interaction
{
    public class LevelingAction : Interaction
    {
        public LevelingType LevelingType { get; }

        public ClassType ClassType { get; }

        public int CurrentLevel { get; }

        public int ClassLevel { get; }

        public LevelingAction(LevelingType levelingType, ClassType classType, int currentLevel, int classLevel) : base(InteractionType.LEVELING)
        {
            LevelingType = levelingType;
            ClassType = classType;
            CurrentLevel = currentLevel;
            ClassLevel = classLevel;
            ParamAdd1 = ((int)levelingType * 100) + (int)classType;
            ParamAdd2 =  ((int)currentLevel * 100) + (int)classLevel;
        }

        public static bool TryGetAdventure(int paramAdd1, int paramAdd2, out LevelingAction leveling)
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

        private LevelingAction(int paramAdd1, int paramAdd2) : base(InteractionType.LEVELING)
        {
            string param1 = paramAdd1.ToString("0000");
            LevelingType = (LevelingType)int.Parse(param1.Substring(0, 2));
            ClassType =  (ClassType)int.Parse(param1.Substring(2, 2));

            string param2 = paramAdd2.ToString("0000");
            CurrentLevel = int.Parse(param2.Substring(0,2));
            ClassLevel = int.Parse(param2.Substring(2,2));

            ParamAdd1 = paramAdd1;
            ParamAdd2 = paramAdd2;
        }

    }
}
