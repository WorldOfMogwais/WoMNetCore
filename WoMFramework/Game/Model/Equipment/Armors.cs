using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public class Armors
    {
        /***
         * Light Armor
         */
        public static Armor StuddedLeather =>
            ArmorBuilder.Create("Studded Leather", ArmorType.LIGHT, 3, 5)
            .SetArmorCheckPenalty(-1)
            .SetArcaneSpellFailureChance(0.15)
            .SetWeight(20)
            .SetCost(25)
            .SetDescription("An improved form of leather armor, studded leather armor is covered " +
                "with dozens of metal protuberances. While these rounded studs offer little defense " +
                "individually, in the numbers they are arrayed in upon such armor, they help catch " +
                "lethal edges and channel them away from vital spots. The rigidity caused by the " +
                "additional metal does, however, result in less mobility than is afforded by a suit " +
                "of normal leather armor.")
            .Build();

        /***
         * Medium Armor
         */
        public static Armor Breastplate =>
            ArmorBuilder.Create("Breastplate", ArmorType.MEDIUM, 6, 3)
            .SetArmorCheckPenalty(-4)
            .SetArcaneSpellFailureChance(0.25)
            .SetWeight(30)
            .SetCost(200)
            .SetDescription("A breastplate protects a wearer’s torso with a single piece of sculpted metal, " +
                "similar to the core piece of a suit of full plate. Despite its sturdiness, its inflexibility " +
                "and open back make it inferior to complete suits of metal armor, but still an improvement " +
                "over most non-metal armors.")
            .Build();

        /***
         * Heaviy Armor
         */
        public static Armor FullPlate =>
            ArmorBuilder.Create("Full Plate", ArmorType.HEAVY, 9, 1)
            .SetArmorCheckPenalty(-6)
            .SetArcaneSpellFailureChance(0.35)
            .SetWeight(50)
            .SetCost(1500)
            .SetDescription("This metal suit comprises multiple pieces of interconnected and overlaying metal " +
                "plates, incorporating the benefits of numerous types of lesser armor. A complete suit of full " +
                "plate (or platemail, as it is often called) includes gauntlets, heavy leather boots, a visored " +
                "helmet, and a thick layer of padding that is worn underneath the armor. Each suit of full plate " +
                "must be individually fitted to its owner by a master armorsmith, although a captured suit can " +
                "be resized to fit a new owner at a cost of 200 to 800 gold pieces.")
            .Build();

    }
}
