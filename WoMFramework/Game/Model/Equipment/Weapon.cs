using System.Collections.Generic;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public sealed class WeaponBuilder
    {
        // description
        private int _criticalMinRoll = 20;
        private int _criticalMultiplier = 2;
        private WeaponDamageType[] _weaponDamageTypes = { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing };
        private int _range = 0;
        private int _cost = 1;
        private double _weight = 1;
        private string _description = string.Empty;

        public string Name;
        public ProficiencyType ProficiencyType;
        public WeaponType WeaponType;
        public int[] DamageSmallRollEvent;
        public int[] DamageMediumRollEvent;

        private WeaponBuilder(string name, ProficiencyType proficiencyType, WeaponType weaponType, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            Name = name;
            ProficiencyType = proficiencyType;
            WeaponType = weaponType;
            DamageSmallRollEvent = damageSmallRollEvent;
            DamageMediumRollEvent = damageMediumRollEvent;
        }
        public static WeaponBuilder Create(string name, ProficiencyType proficiencyType, WeaponType weaponType, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            return new WeaponBuilder(name, proficiencyType, weaponType, damageSmallRollEvent, damageMediumRollEvent);
        }
        public WeaponBuilder SetCriticalMinRoll(int criticalMinRoll)
        {
            _criticalMinRoll = criticalMinRoll;
            return this;
        }
        public WeaponBuilder SetCriticalMultiplier(int criticalMultiplier)
        {
            _criticalMultiplier = criticalMultiplier;
            return this;
        }
        public WeaponBuilder SetDamageType(WeaponDamageType weaponDamageType)
        {
            _weaponDamageTypes = new[] { weaponDamageType };
            return this;
        }
        public WeaponBuilder SetDamageTypes(WeaponDamageType[] weaponDamageTypes)
        {
            _weaponDamageTypes = weaponDamageTypes;
            return this;
        }
        public WeaponBuilder SetRange(int range)
        {
            _range = range;
            return this;
        }
        public WeaponBuilder SetCost(int cost)
        {
            _cost = cost;
            return this;
        }
        public WeaponBuilder SetWeight(double weight)
        {
            _weight = weight;
            return this;
        }
        public WeaponBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }
        public Weapon Build()
        {
            return new Weapon(Name, ProficiencyType, WeaponType, DamageSmallRollEvent, DamageMediumRollEvent, _criticalMinRoll, _criticalMultiplier, _weaponDamageTypes, _range, _cost, _weight, _description);
        }
    }
    public class NaturalWeapon
    {
        private static readonly Dictionary<SizeType, int[]> BiteDic = new Dictionary<SizeType, int[]>
        {
            { SizeType.Diminutive, new[] {1, 2} },
            { SizeType.Tiny, new[] {1, 3} },
            { SizeType.Small, new[] {1, 4} },
            { SizeType.Medium, new[] {1, 6} },
            { SizeType.Large, new[] {1, 8} },
            { SizeType.Huge, new[] {2, 6} },
            { SizeType.Gargantuan, new[] {2, 8} },
            { SizeType.Colossal, new[] {4, 6} }
        };

        public static Weapon Bite(SizeType sizeType)
        {
            return new Weapon("Bite", ProficiencyType.Simple, WeaponType.Unarmed, BiteDic[sizeType], 20, 2, new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing }, 1, 1, 0, "");
        }
    }
    public class Weapon : BaseItem
    {
        public ProficiencyType ProficiencyType { get; }
        public WeaponType WeaponType { get; }
        public int[] DamageRoll { get; set; }
        public int CriticalMinRoll { get; }
        public int CriticalMultiplier { get; }
        public WeaponDamageType[] WeaponDamageTypes { get; }
        public int Range { get; }

        public int MinDmg => DamageRoll[0] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);
        public int MaxDmg => DamageRoll[0] * DamageRoll[1] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);

        private readonly int[] _damageSmallRollEvent;
        public Weapon Small
        {
            get
            {
                if (_damageSmallRollEvent != null)
                {
                    DamageRoll = _damageSmallRollEvent;
                }
                return this;
            }
        }

        public bool IsTwoHanded => WeaponType == WeaponType.TwoHanded;

        public bool IsCriticalRoll(int roll) => roll >= CriticalMinRoll;

        public Weapon(string name, ProficiencyType proficiencyType,  WeaponType weaponType, int[] damageRoll, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, int cost, double weight, string description) : base(name, cost, weight, description)
        {
            ProficiencyType = proficiencyType;
            WeaponType = weaponType;
            DamageRoll = damageRoll;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
        }

        public Weapon(string name,  ProficiencyType proficiencyType,  WeaponType weaponType, int[] damageSmallRollEvent, int[] damageMediumRollEvent, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, int cost, double weight, string description) : base(name, cost, weight, description)
        {
            ProficiencyType = proficiencyType;
            WeaponType = weaponType;
            DamageRoll = damageMediumRollEvent;
            _damageSmallRollEvent = damageSmallRollEvent;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
        }

    }

    public enum ProficiencyType
    {
        Simple,
        Martial,
        Exotic
    }

    public enum WeaponType
    {
        Unarmed,
        Light,
        OneHanded,
        TwoHanded,
        Ranged,
        Ammunition
    }
}