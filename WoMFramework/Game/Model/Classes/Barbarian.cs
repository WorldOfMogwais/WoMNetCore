using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public class Barbarian : Classes
    {
        public Barbarian() : base(ClassType.BARBARIAN)
        {
            HitHitPointDiceRollEvent = new int[] { 1, 12 };
            WealthDiceRollEvent = new int[] { 3, 6 , 0, 1};
            Description = "For some, there is only rage. In the ways of their people, in the fury of their passion, " +
                "in the howl of battle, conflict is all these brutal souls know. Savages, hired muscle, masters of " +
                "vicious martial techniques, they are not soldiers or professional warriors—they are the battle possessed, " +
                "creatures of slaughter and spirits of war. Known as barbarians, these warmongers know little of training, " +
                "preparation, or the rules of warfare; for them, only the moment exists, with the foes that stand before " +
                "them and the knowledge that the next moment might hold their death. They possess a sixth sense in regard " +
                "to danger and the endurance to weather all that might entail. These brutal warriors might rise from all " +
                "walks of life, both civilized and savage, though whole societies embracing such philosophies roam the wild " +
                "places of the world. Within barbarians storms the primal spirit of battle, and woe to those who face their rage.";
            Role = "Barbarians excel in combat, possessing the martial prowess and fortitude to take on foes seemingly far " +
                "superior to themselves. With rage granting them boldness and daring beyond that of most other warriors, " +
                "barbarians charge furiously into battle and ruin all who would stand in their way.";
        }

        public override void ClassLevelUp()
        {
            base.ClassLevelUp();

            FortitudeBaseSave = (int) (2+((double)ClassLevel/2));
            ReflexBaseSave = (int) (0+((double)ClassLevel/3));
            WillBaseSave = (int)  (0+((double)ClassLevel/3));

            AddBaseAttackBonus(1);
        }
    }
}
