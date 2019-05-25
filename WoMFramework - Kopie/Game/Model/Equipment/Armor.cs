using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public sealed class ArmorBuilder
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

    public class Armor : BaseItem
    {
        public ArmorEffortType ArmorEffortType { get; }
        public int ArmorBonus { get; }
        public int? MaxDexterityBonus { get; }
        public int ArmorCheckPenalty { get; }
        public double ArcaneSpellFailureChance { get; }

        public int? SpeedReduction30Ft { get; }
        public int? SpeedReduction20Ft { get; }

        public Armor(string name, ArmorEffortType armorEffortType, int armorBonus, int? maxDexterityBonus, int armorCheckPenalty, double arcaneSpellFailureChance, int? speedReduction30Ft, int? speedReduction20Ft, double cost, double weight, string description) : base(name, cost, weight, description, slotType:armorEffortType == ArmorEffortType.Shield ? SlotType.Weapon : SlotType.Armor)
        {
            ArmorEffortType = armorEffortType;
            ArmorBonus = armorBonus;
            MaxDexterityBonus = maxDexterityBonus;
            ArmorCheckPenalty = armorCheckPenalty;
            ArcaneSpellFailureChance = arcaneSpellFailureChance;
            SpeedReduction30Ft = speedReduction30Ft;
            SpeedReduction20Ft = speedReduction20Ft;
        }
    }
}
