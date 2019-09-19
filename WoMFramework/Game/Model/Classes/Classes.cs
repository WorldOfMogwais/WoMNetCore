namespace WoMFramework.Game.Model.Classes
{
    using Enums;
    using Learnable;
    using System;
    using System.Collections.Generic;

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

        public bool CanCast { get; set; }

        public List<ILearnable> Learnables;

        public abstract int CasterMod(Entity entity);

        public abstract List<Spell> ClassSpells();

        protected Classes(ClassType classType, bool caster)
        {
            ClassType = classType;
            ClassLevel = 0;
            ClassAttackBonus = 0;
            CanCast = caster;

            Learnables = new List<ILearnable>();
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
                case ClassType.Bard:
                    return new Bard();
                case ClassType.Cleric:
                    return new Cleric();
                case ClassType.Druid:
                    return new Druid();
                case ClassType.Fighter:
                    return new Fighter();
                case ClassType.Monk:
                    return new Monk();
                case ClassType.Paladin:
                    return new Paladin();
                case ClassType.Ranger:
                    return new Ranger();
                case ClassType.Rogue:
                    return new Rogue();
                case ClassType.Sorcerer:
                    return new Sorcerer();
                case ClassType.Wizard:
                    return new Wizard();
                case ClassType.None:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException(nameof(classType), classType, null);
            }
        }
    }
}
