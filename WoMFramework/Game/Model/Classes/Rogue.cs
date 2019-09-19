namespace WoMFramework.Game.Model.Classes
{
    using Enums;
    using Learnable;
    using System.Collections.Generic;

    public class Rogue : Classes
    {
        public Rogue() : base(ClassType.Rogue, false)
        {
            HitPointDiceRollEvent = new[] { 1, 8 };
            WealthDiceRollEvent = new[] { 3, 6, 0, 1 };
            Description = "Life is an endless adventure for those who live by their wits. Ever just one step ahead of danger, rogues bank on their cunning, skill, and charm to bend fate to their favor. Never knowing what to expect, they prepare for everything, becoming masters of a wide variety of skills, training themselves to be adept manipulators, agile acrobats, shadowy stalkers, or masters of any of dozens of other professions or talents. Thieves and gamblers, fast talkers and diplomats, bandits and bounty hunters, and explorers and investigators all might be considered rogues, as well as countless other professions that rely upon wits, prowess, or luck. Although many rogues favor cities and the innumerable opportunities of civilization, some embrace lives on the road, journeying far, meeting exotic people, and facing fantastic danger in pursuit of equally fantastic riches. In the end, any who desire to shape their fates and live life on their own terms might come to be called rogues.";
            Role = "Rogues excel at moving about unseen and catching foes unaware, and tend to avoid head-to-head combat. Their varied skills and abilities allow them to be highly versatile, with great variations in expertise existing between different rogues. Most, however, excel in overcoming hindrances of all types, from unlocking doors and disarming traps to outwitting magical hazards and conning dull-witted opponents.";
            //Alignment: Any
            Learnables.AddRange(ClassSpells());
        }

        public override int CasterMod(Entity entity) => int.MinValue;

        public override List<Spell> ClassSpells()
        {
            return new List<Spell>()
            {
            };
        }

        public override void ClassLevelUp()
        {
            base.ClassLevelUp();

            FortitudeBaseSave = (int)(0 + (double)ClassLevel / 3);
            ReflexBaseSave = (int)(2 + (double)ClassLevel / 2);
            WillBaseSave = (int)(0 + (double)ClassLevel / 3);

            ClassAttackBonus = (ClassLevel - 1) - (int)((double)(ClassLevel - 1) / 4);
        }
    }
}