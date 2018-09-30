using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public sealed class ArmorBuilder
    {
        // description
        private int _armorCheckPenalty;
        private double _arcaneSpellFailureChance;
        private double _cost = 1;
        private double _weight = 1;
        private string _description = string.Empty;
        private int? _speedReduction30Ft = null;
        private int? _speedReduction20Ft = null;

        public string Name;
        public ArmorEffortType ArmorEffortType;
        public int ArmorBonus;
        public int? MaxDexterityBonus;

        private ArmorBuilder(string name, ArmorEffortType armorEffortType, int armorBonus, int? maxDexterityBonus)
        {
            Name = name;
            ArmorEffortType = armorEffortType;
            ArmorBonus = armorBonus;
            MaxDexterityBonus = maxDexterityBonus;
        }
        public static ArmorBuilder Create(string name, ArmorEffortType armorEffortType, int armorBonus, int? maxDexterityBonus)
        {
            return new ArmorBuilder(name, armorEffortType, armorBonus, maxDexterityBonus);
        }
        public ArmorBuilder SetArmorCheckPenalty(int armorCheckPenalty)
        {
            _armorCheckPenalty = armorCheckPenalty;
            return this;
        }
        public ArmorBuilder SetArcaneSpellFailureChance(double arcaneSpellFailureChance)
        {
            _arcaneSpellFailureChance = arcaneSpellFailureChance;
            return this;
        }
        public ArmorBuilder SetSpeedReductions(int? speedReduction30Ft, int? speedReduction20Ft)
        {
            _speedReduction30Ft = speedReduction30Ft;
            _speedReduction30Ft = speedReduction20Ft;
            return this;
        }
        public ArmorBuilder SetCost(double cost)
        {
            _cost = cost;
            return this;
        }
        public ArmorBuilder SetWeight(double weight)
        {
            _weight = weight;
            return this;
        }
        public ArmorBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }
        public Armor Build()
        {
            return new Armor(Name, ArmorEffortType, ArmorBonus, MaxDexterityBonus, _armorCheckPenalty, _arcaneSpellFailureChance, _speedReduction30Ft, _speedReduction20Ft, _cost, _weight, _description);
        }
    }

    public class Armor : BaseItem
    {
        public ArmorEffortType ArmorEffortType { get; }
        public int ArmorBonus { get; }
        public int? MaxDexterityBonus { get; }
        public int ArmorCheckPenalty { get; }
        public double ArcaneSpellFailureChance { get; }

        public int? SpeedReduction30ft { get; }
        public int? SpeedReduction20ft { get; }

        public Armor(string name, ArmorEffortType armorEffortType, int armorBonus, int? maxDexterityBonus, int armorCheckPenalty, double arcaneSpellFailureChance, int? speedReduction30Ft, int? speedReduction20Ft, double cost, double weight, string description) : base(name, cost, weight, description)
        {
            ArmorEffortType = armorEffortType;
            ArmorBonus = armorBonus;
            MaxDexterityBonus = maxDexterityBonus;
            ArmorCheckPenalty = armorCheckPenalty;
            ArcaneSpellFailureChance = arcaneSpellFailureChance;
            SpeedReduction30ft = speedReduction30Ft;
            SpeedReduction20ft = speedReduction20Ft;
        }
    }
}
