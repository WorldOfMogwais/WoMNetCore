using System.Collections.Generic;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public abstract class Classes
    {
        public string Name => ClassType.ToString();

        public ClassType ClassType { get; set; }
        public int ClassLevel { get; set; }

        public int FortitudeBaseSave { get; set; }
        public int ReflexBaseSave { get; set; }
        public int WillBaseSave { get; set; }

        public int ClassAttackBonus { get; set; }

        public int[] HitPointDiceRollEvent { get; set; }

        public int[] WealthDiceRollEvent { get; set; }

        public string Description { get; set; }
        public string Role { get; set; }

        protected Classes(ClassType classType)
        {
            ClassType = classType;
            ClassLevel = 0;
            ClassAttackBonus = 0;
        }

        public virtual void ClassLevelUp()
        {
            ClassLevel += 1;
        }

        public static Classes GetClasses(ClassType classType)
        {
            switch (classType)
            {
                case ClassType.Barbarian:
                    return new Barbarian();
                default:
                    return new NoClass();
            }
        }

    }
}
