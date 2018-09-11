using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public sealed class ArmorBuilder
    {
        // description
        private int _armorCheckPenalty = 0;
        private double _arcaneSpellFailureChance = 0;
        private int _cost = 1;
        private int _weight = 1;
        private string _description = string.Empty;

        public string Name;
        public ArmorType ArmorType;
        public int ArmorBonus;
        public int MaxDexterityBonus;

        private ArmorBuilder(string name, ArmorType armorType, int armorBonus, int maxDexterityBonus)
        {
            this.Name = name;
            this.ArmorType = armorType;
            this.ArmorBonus = armorBonus;
            this.MaxDexterityBonus = maxDexterityBonus;
        }
        public static ArmorBuilder Create(string name, ArmorType armorType, int armorBonus, int maxDexterityBonus)
        {
            return new ArmorBuilder(name, armorType, armorBonus, maxDexterityBonus);
        }
        public ArmorBuilder SetArmorCheckPenalty(int armorCheckPenalty)
        {
            this._armorCheckPenalty = armorCheckPenalty;
            return this;
        }
        public ArmorBuilder SetArcaneSpellFailureChance(double arcaneSpellFailureChance)
        {
            this._arcaneSpellFailureChance = arcaneSpellFailureChance;
            return this;
        }
        public ArmorBuilder SetCost(int cost)
        {
            this._cost = cost;
            return this;
        }
        public ArmorBuilder SetWeight(int weight)
        {
            this._weight = weight;
            return this;
        }
        public ArmorBuilder SetDescription(string description)
        {
            this._description = description;
            return this;
        }
        public Armor Build()
        {
            return new Armor(Name, ArmorType, ArmorBonus, MaxDexterityBonus, _armorCheckPenalty, _arcaneSpellFailureChance, _cost, _weight, _description);
        }
    }

    public class Armor : BaseItem
    {
        public ArmorType ArmorType { get; }
        public int ArmorBonus { get; }
        public int MaxDexterityBonus { get; }
        public int ArmorCheckPenalty { get; }
        public double ArcaneSpellFailureChance { get; }

        public Armor(string name, ArmorType armorType, int armorBonus, int maxDexterityBonus, int armorCheckPenalty, double arcaneSpellFailureChance, int cost, int weight, string description) : base(name, cost, weight, description)
        {
            ArmorType = armorType;
            ArmorBonus = armorBonus;
            MaxDexterityBonus = maxDexterityBonus;
            ArmorCheckPenalty = armorCheckPenalty;
            ArcaneSpellFailureChance = arcaneSpellFailureChance;
        }
    }
}
