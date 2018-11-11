using System.Collections.Generic;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Classes
{
    public class Druid : Classes
    {
        public Druid() : base(ClassType.Druid, true)
        {
            HitPointDiceRollEvent = new[] { 1, 8 };
            WealthDiceRollEvent = new[] { 3, 6, 0, 1 };
            Description = "Within the purity of the elements and the order of the wilds lingers a power beyond the marvels of civilization. Furtive yet undeniable, these primal magics are guarded over by servants of philosophical balance known as druids. Allies to beasts and manipulators of nature, these often misunderstood protectors of the wild strive to shield their lands from all who would threaten them and prove the might of the wilds to those who lock themselves behind city walls. Rewarded for their devotion with incredible powers, druids gain unparalleled shape-shifting abilities, the companionship of mighty beasts, and the power to call upon nature's wrath. The mightiest temper powers akin to storms, earthquakes, and volcanoes with primeval wisdom long abandoned and forgotten by civilization.";
            Role = "While some druids might keep to the fringe of battle, allowing companions and summoned creatures to fight while they confound foes with the powers of nature, others transform into deadly beasts and savagely wade into combat. Druids worship personifications of elemental forces, natural powers, or nature itself. Typically this means devotion to a nature deity, though druids are just as likely to revere vague spirits, animalistic demigods, or even specific awe-inspiring natural wonders.";
            //Alignment: Any neutral
            Learnables.AddRange(ClassSpells());
        }

        public override int CasterMod(Entity entity) => entity.WisdomMod;

        public override List<Spell> ClassSpells()
        {
            return new List<Spell>()
            {
            };
        }

        public override void ClassLevelUp()
        {
            base.ClassLevelUp();

            FortitudeBaseSave = (int)(2 + (double)ClassLevel / 2);
            ReflexBaseSave = (int)(0 + (double)ClassLevel / 3);
            WillBaseSave = (int)(2 + (double)ClassLevel / 2);

            ClassAttackBonus = (ClassLevel - 1) - (int)((double)(ClassLevel - 1) / 4);
        }
    }
}