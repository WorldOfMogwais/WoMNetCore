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
        private double _cost = 1;
        private double _weight = 1;
        private string _description = string.Empty;

        public string Name;
        public WeaponProficiencyType WeaponProficiencyType;
        public WeaponEffortType WeaponEffortType;
        public int[] DamageSmallRollEvent;
        public int[] DamageMediumRollEvent;

        private WeaponBuilder(string name, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            Name = name;
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            DamageSmallRollEvent = damageSmallRollEvent;
            DamageMediumRollEvent = damageMediumRollEvent;
        }
        public static WeaponBuilder Create(string name, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            return new WeaponBuilder(name, weaponProficiencyType, weaponEffortType, damageSmallRollEvent, damageMediumRollEvent);
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
        public WeaponBuilder SetCost(double cost)
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
            return new Weapon(Name, WeaponProficiencyType, WeaponEffortType, DamageSmallRollEvent, DamageMediumRollEvent, _criticalMinRoll, _criticalMultiplier, _weaponDamageTypes, _range, _cost, _weight, _description);
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
        public static Weapon Bite(SizeType sizeType) => new Weapon("Bite", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, BiteDic[sizeType], 20, 2, new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing }, 1, 1, 0, "");
    }

    public class Weapon : BaseItem
    {
        public WeaponProficiencyType WeaponProficiencyType { get; }
        public WeaponEffortType WeaponEffortType { get; }
        public int[] DamageRoll { get; set; }
        public int[] SmallDamageRollEvent { get; }
        public int CriticalMinRoll { get; }
        public int CriticalMultiplier { get; }
        public WeaponDamageType[] WeaponDamageTypes { get; }
        public int Range { get; }

        public int MinDmg => DamageRoll[0] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);
        public int MaxDmg => DamageRoll[0] * DamageRoll[1] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);

        public bool IsCriticalRoll(int roll) => roll >= CriticalMinRoll;

        protected Weapon Small
        {
            get
            {
                if (SmallDamageRollEvent != null)
                {
                    DamageRoll = SmallDamageRollEvent;
                }
                return this;
            }
        }

        public Weapon(string name, WeaponProficiencyType weaponProficiencyType,  WeaponEffortType weaponEffortType, int[] damageRoll, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, double cost, double weight, string description) : base(name, cost, weight, description)
        {
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            DamageRoll = damageRoll;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
        }

        public Weapon(string name,  WeaponProficiencyType weaponProficiencyType,  WeaponEffortType weaponEffortType, int[] smallDamageRollEvent, int[] damageMediumRollEvent, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, double cost, double weight, string description) : base(name, cost, weight, description)
        {
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            DamageRoll = damageMediumRollEvent;
            SmallDamageRollEvent = smallDamageRollEvent;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
        }

    }
}