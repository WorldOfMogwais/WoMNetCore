using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public abstract class Classes
    {
        public string Name => ClassType.ToString().Substring(0, 1) + ClassType.ToString().Substring(1).ToLower();

        public ClassType ClassType { get; set; }
        public int ClassLevel { get; set; }

        public int FortitudeBaseSave { get; set; }
        public int ReflexBaseSave { get; set; }
        public int WillBaseSave { get; set; }

        public int[] BaseAttackBonus { get; set; }

        public int[] HitHitPointDiceRollEvent { get; set; }

        public int[] WealthDiceRollEvent { get; set; }

        public string Description { get; set; }
        public string Role { get; set; }

        public Classes(ClassType ClassType)
        {
            ClassLevel = 0;
            BaseAttackBonus = new int[] { 0 };
        }

        internal void AddBaseAttackBonus(int value)
        {
            int currentBaseAttackBonus = BaseAttackBonus[0] + value;

            var baseAttackBonusList = new List<int>();

            for (int i = currentBaseAttackBonus; i > 0; i = i - 5) {
                baseAttackBonusList.Add(i);
            }
            BaseAttackBonus = baseAttackBonusList.ToArray();
        }

        public virtual void ClassLevelUp()
        {
            ClassLevel += 1;
        }

    }
}
