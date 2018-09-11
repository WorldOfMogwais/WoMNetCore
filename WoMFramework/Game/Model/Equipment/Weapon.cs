using System;
using System.Collections.Generic;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public sealed class WeaponBuilder
    {
        // description
        private bool isTwoHanded = false;
        private int criticalMinRoll = 20;
        private int criticalMultiplier = 2;
        private WeaponDamageType[] weaponDamageTypes = new WeaponDamageType[] { WeaponDamageType.BLUDGEONING, WeaponDamageType.PIERCING, WeaponDamageType.SLASHING };
        private int range = 1;
        private int cost = 1;
        private double weight = 1;
        private string description = string.Empty;

        public string name;
        public int[] damageSmallRollEvent;
        public int[] damageMediumRollEvent;

        private WeaponBuilder(string name, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            this.name = name;
            this.damageSmallRollEvent = damageSmallRollEvent;
            this.damageMediumRollEvent = damageMediumRollEvent;
        }
        public static WeaponBuilder Create(string name, int[] damageSmallRollEvent, int[] damageMediumRollEvent)
        {
            return new WeaponBuilder(name, damageSmallRollEvent, damageMediumRollEvent);
        }
        public WeaponBuilder SetCriticalMinRoll(int criticalMinRoll)
        {
            this.criticalMinRoll = criticalMinRoll;
            return this;
        }
        public WeaponBuilder IsTwoHanded()
        {
            isTwoHanded = true;
            return this;
        }
        public WeaponBuilder SetCriticalMultiplier(int criticalMultiplier)
        {
            this.criticalMultiplier = criticalMultiplier;
            return this;
        }
        public WeaponBuilder SetDamageType(WeaponDamageType weaponDamageType)
        {
            this.weaponDamageTypes = new WeaponDamageType[] { weaponDamageType };
            return this;
        }
        public WeaponBuilder SetDamageTypes(WeaponDamageType[] weaponDamageTypes)
        {
            this.weaponDamageTypes = weaponDamageTypes;
            return this;
        }
        public WeaponBuilder SetRange(int range)
        {
            this.range = range;
            return this;
        }
        public WeaponBuilder SetCost(int cost)
        {
            this.cost = cost;
            return this;
        }
        public WeaponBuilder SetWeight(double weight)
        {
            this.weight = weight;
            return this;
        }
        public WeaponBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }
        public Weapon Build()
        {
            return new Weapon(name, damageSmallRollEvent, damageMediumRollEvent, criticalMinRoll, criticalMultiplier, weaponDamageTypes, range, isTwoHanded, cost, weight, description);
        }
    }
    public class NaturalWeapon
    {
        private static Dictionary<SizeType, int[]> BiteDic = new Dictionary<SizeType, int[]>() {
            { SizeType.DIMINUTIVE, new int[] {1, 2} },
            { SizeType.TINY, new int[] {1, 3} },
            { SizeType.SMALL, new int[] {1, 4} },
            { SizeType.MEDIUM, new int[] {1, 6} },
            { SizeType.LARGE, new int[] {1, 8} },
            { SizeType.HUGE, new int[] {2, 6} },
            { SizeType.GARGANTUAN, new int[] {2, 8} },
            { SizeType.COLOSSAL, new int[] {4, 6} },
        };

        public static Weapon Bite(SizeType sizeType)
        {
            return new Weapon("Bite", BiteDic[sizeType], 20, 2, new WeaponDamageType[] { WeaponDamageType.BLUDGEONING, WeaponDamageType.PIERCING, WeaponDamageType.SLASHING }, 1, false, 1, 0, "");
        }
    }
    public class Weapon : BaseItem
    {
        public int[] DamageRoll { get; set; }
        private int[] damageSmallRollEvent;

        public int CriticalMinRoll { get; }
        public int CriticalMultiplier { get; }
        public WeaponDamageType[] WeaponDamageTypes { get; }
        public int Range { get; }
        public bool IsCriticalRoll(int roll) => roll >= CriticalMinRoll;
        public bool IsTwoHanded { get; }

        public int MinDmg => DamageRoll[0] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);
        public int MaxDmg => (DamageRoll[0] * DamageRoll[1]) + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);

        public Weapon Small
        {
            get
            {
                if (damageSmallRollEvent != null)
                {
                    DamageRoll = damageSmallRollEvent;
                }
                return this;
            }
        }

        public Weapon(string name, int[] damageRoll, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, bool isTwoHanded, int cost, double weight, string description) : base(name, cost, weight, description)
        {
            DamageRoll = damageRoll;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
            IsTwoHanded = isTwoHanded;
        }

        public Weapon(string name, int[] damageSmallRollEvent, int[] damageMediumRollEvent, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, bool isTwoHanded, int cost, double weight, string description) : base(name, cost, weight, description)
        {
            DamageRoll = damageMediumRollEvent;
            this.damageSmallRollEvent = damageSmallRollEvent;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
            IsTwoHanded = isTwoHanded;
        }

    }

}