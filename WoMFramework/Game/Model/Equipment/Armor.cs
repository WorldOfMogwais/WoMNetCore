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
        private int armorCheckPenalty = 0;
        private double arcaneSpellFailureChance = 0;
        private int cost = 1;
        private int weight = 1;
        private string description = string.Empty;

        public string name;
        public ArmorType armorType;
        public int armorBonus;
        public int maxDexterityBonus;

        private ArmorBuilder(string name, ArmorType armorType, int armorBonus, int maxDexterityBonus)
        {
            this.name = name;
            this.armorType = armorType;
            this.armorBonus = armorBonus;
            this.maxDexterityBonus = maxDexterityBonus;
        }
        public static ArmorBuilder Create(string name, ArmorType armorType, int armorBonus, int maxDexterityBonus)
        {
            return new ArmorBuilder(name, armorType, armorBonus, maxDexterityBonus);
        }
        public ArmorBuilder SetArmorCheckPenalty(int armorCheckPenalty)
        {
            this.armorCheckPenalty = armorCheckPenalty;
            return this;
        }
        public ArmorBuilder SetArcaneSpellFailureChance(double arcaneSpellFailureChance)
        {
            this.arcaneSpellFailureChance = arcaneSpellFailureChance;
            return this;
        }
        public ArmorBuilder SetCost(int cost)
        {
            this.cost = cost;
            return this;
        }
        public ArmorBuilder SetWeight(int weight)
        {
            this.weight = weight;
            return this;
        }
        public ArmorBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }
        public Armor Build()
        {
            return new Armor(name, armorType, armorBonus, maxDexterityBonus, armorCheckPenalty, arcaneSpellFailureChance, cost, weight, description);
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
