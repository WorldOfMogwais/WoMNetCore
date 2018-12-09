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

        public string Name  { get; set; }
        
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


}
