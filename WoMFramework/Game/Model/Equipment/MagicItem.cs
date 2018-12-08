using System;
using System.Collections.Generic;
using System.Text;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public sealed class MagicItemBuilder
    {
        // description
        public int ArmorCheckPenalty { get; set; }
        public double ArcaneSpellFailureChance { get; set; }
        public double Cost { get; set; }
        public double Weight { get; set; }
        public string Description = string.Empty;
        public int? SpeedReduction30Ft = null;
        public int? SpeedReduction20Ft = null;

        public string Name  { get; set; }
        public ArmorEffortType ArmorEffortType { get; set; }
        public int ArmorBonus { get; set; }
        public int? MaxDexterityBonus { get; set; }

        public Armor Build()
        {
            return new Armor(Name, ArmorEffortType, ArmorBonus, MaxDexterityBonus, ArmorCheckPenalty, ArcaneSpellFailureChance, SpeedReduction30Ft, SpeedReduction20Ft, Cost, Weight, Description);
        }
    }

    public class MagicItem : BaseItem
    {
        public MagicItem(string name, double cost, double weight, string description, RarityType rarityType = RarityType.Normal, SlotType slotType = SlotType.None) : base(name, cost, weight, description, rarityType, slotType)
        {

        }
    }
}
