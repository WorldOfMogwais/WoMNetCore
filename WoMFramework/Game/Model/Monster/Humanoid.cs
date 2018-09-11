using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public partial class Monsters
    {
        public static Monster Kobold =>
            MonsterBuilder.Create("Kobold", 0.25, MonsterType.HUMANOIDS, 100)
            .SetSizeType(SizeType.SMALL)
            .SetAbilities(9, 13, 10, 10, 9, 8)
            .SetNaturalArmor(1)
            .SetHitPointDiceRollEvent(new int[] { 1, 10 })
            .SetBaseAttackBonus(1)
            .SetBaseWeapon(Weapons.Spear.Small)
            .SetDescription("Kobolds are creatures of the dark, found most commonly in enormous underground warrens " +
                "or the dark corners of the forest where the sun is unable to reach.Due to their physical similarities, " +
                "kobolds loudly proclaim themselves the scions of dragonkind, destined to rule the earth beneath the " +
                "wings of their great god-cousins, but most dragons have little use for the obnoxious pests."+
                "While they may speak loudly of divine right and manifest destiny, kobolds are keenly aware of their " +
                "own weakness.Cowards and schemers, they never fight fair if they can help it, instead setting up ambushes " +
                "and double-crosses, holing up in their warrens behind countless crude but ingenious traps, or rolling " +
                "over the enemy in vast, yipping hordes." +
                "Kobold coloration varies even among siblings from the same egg clutch, ranging through the colors of the " +
                "chromatic dragons, with red being the most common but white, green, blue, and black kobolds not unheard of.")
            .Build();
    }
}
