using System;
using System.Collections.Generic;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{
    public sealed class MagicItemBuilder
    {
        // description
        public double Cost { get; set; }
        public double Weight { get; set; }
        public string Description = string.Empty;

        public string Name { get; set; }

        public List<Modifier> Modifiers { get; set; }

        public List<CombatAction> CombatActions { get; set; }

        public MagicItem Build()
        {
            return new MagicItem(Name, Modifiers, CombatActions, Cost, Weight, Description);
        }
    }

    public class MagicItem : BaseItem
    {
        public MagicItem(string name, List<Modifier> modifier, List<CombatAction> combatActions, double cost, double weight, string description, RarityType rarityType = RarityType.Normal, SlotType slotType = SlotType.None) : base(name, cost, weight, description, rarityType, slotType)
        {
            Modifiers = modifier;
            CombatActions = combatActions;
        }
    }

    public class MagicItems
    {
        public static MagicItem BoneOfMogwan()
        {
            return new MagicItem("Bone of Mogwan",
                new List<Modifier>()
                {
                    new SimpleModifier(ModifierType.Strength, (Entity) => 2),
                    new SimpleModifier(ModifierType.Dexterity, (Entity) => 2),
                    new SimpleModifier(ModifierType.Constitution, (Entity) => 2),
                    new SimpleModifier(ModifierType.Inteligence, (Entity) => 2),
                    new SimpleModifier(ModifierType.Wisdom, (Entity) => 2),
                    new SimpleModifier(ModifierType.Charisma, (Entity) => 2),
                    new SimpleModifier(ModifierType.ArmorClass, (Entity) => 1),
                    new SimpleModifier(ModifierType.Initiative, (Entity) => 1),
                    new SimpleModifier(ModifierType.AttackBonus, (Entity) => 1),
                    new SimpleModifier(ModifierType.Speed, (Entity) => 1),
                    new SimpleModifier(ModifierType.Will, (Entity) => 1),
                    new SimpleModifier(ModifierType.Reflex, (Entity) => 1),
                    new SimpleModifier(ModifierType.Fortitude, (Entity) => 1),
                },
                new List<CombatAction>(),
                1000D,
                0.1D,
                "One of the most wanted items that ever existed in the World of Mogwais.",
                RarityType.Legendary,
                SlotType.Ring
                );
        }

        public static MagicItem RingOfTheBear()
        {
            return new MagicItem("Ring of the Bear",
                new List<Modifier>()
                {
                    new SimpleModifier(ModifierType.Strength, (Entity) => 2)
                },
                new List<CombatAction>(),
                1D,
                0.1D,
                "This ring somehow makes you feel stronger.",
                RarityType.Magic,
                SlotType.Ring
            );
        }
    }


}
