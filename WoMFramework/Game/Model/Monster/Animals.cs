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
        public static Monster Rat =>
            MonsterBuilder.Create("Rat", 0.25, MonsterType.ANIMALS, 100)
             .SetSizeType(SizeType.TINY)
             .SetAbilities(2, 15, 11, 2, 13, 2)
             .SetBaseSpeed(15)
             .SetHitPointDiceRollEvent(new int[] { 1, 8 })
             .SetBaseAttackBonus(0)
             .SetBaseWeapon(NaturalWeapon.Bite(SizeType.TINY))
             .SetDescription("Fecund and secretive, rats are omnivorous rodents that particularly thrive in urban areas.")
             .Build();

        public static Monster Wolf =>
            MonsterBuilder.Create("Wolf", 1, MonsterType.ANIMALS, 400)
            .SetSizeType(SizeType.MEDIUM)
            .SetAbilities(13, 15, 15, 2, 12, 6)
            .SetBaseSpeed(50)
            .SetNaturalArmor(2)
            .SetHitPointDiceRollEvent(new int[] { 2, 8, 0, 4 })
            .SetBaseAttackBonus(1)
            .SetBaseWeapon(NaturalWeapon.Bite(SizeType.MEDIUM))
            .SetDescription("Wandering alone or in packs, wolves sit at the top of the food chain. Ferociously " +
                "territorial and exceptionally wide-ranging in their hunting, wolf packs cover broad " +
                "areas. A wolf’s wide paws contain slight webbing between the toes that assists in " +
                "moving over snow, and its fur is a thick, water-resistant coat ranging in color from " +
                "gray to brown and even black in some species. Its paws contain scent glands that mark " +
                "the ground as it travels, assisting in navigation as well as broadcasting its whereabouts " +
                "to fellow pack members. Generally, a wolf stands from 2-1/2 to 3 feet tall at the shoulder " +
                "and weighs between 45 and 150 pounds, with females being slightly smaller.")
            .Build();
    }
}
