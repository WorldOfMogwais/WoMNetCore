using System;
using System.Collections.Generic;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
{
    public sealed class WeaponBuilder
    {
        // description
        private WeaponAttackType _weaponAttackType = WeaponAttackType.Primary;
        private int _criticalMinRoll = 20;
        private int _criticalMultiplier = 2;
        private WeaponDamageType[] _weaponDamageTypes = { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing };
        private int _range = 1;
        private SizeType _sizeType = SizeType.Medium;
        private double _cost = 1;
        private double _weight = 1;
        private string _description = string.Empty;

        public string Name;
        public WeaponProficiencyType WeaponProficiencyType;
        public WeaponEffortType WeaponEffortType;
        public int[] DamageMediumRollEvent;

        private WeaponBuilder(string name, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] damageMediumRollEvent)
        {
            Name = name;
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            DamageMediumRollEvent = damageMediumRollEvent;
        }
        public static WeaponBuilder Create(string name, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] damageMediumRollEvent)
        {
            return new WeaponBuilder(name, weaponProficiencyType, weaponEffortType, damageMediumRollEvent);
        }
        public WeaponBuilder SetWeaponAttackType(WeaponAttackType weaponAttackType)
        {
            _weaponAttackType = weaponAttackType;
            return this;
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
        public WeaponBuilder SetSizeType(SizeType sizeType)
        {
            _sizeType = sizeType;
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
            return new Weapon(Name, WeaponProficiencyType, WeaponEffortType, DamageMediumRollEvent,  _weaponAttackType, _criticalMinRoll, _criticalMultiplier, _weaponDamageTypes, _range,_sizeType, _cost, _weight, _description);
        }
    }
    public class NaturalWeapon
    {
        public static Weapon Bite(SizeType sizeType) => 
            WeaponBuilder.Create("Bite", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 6 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Claw(SizeType sizeType) => 
            WeaponBuilder.Create("Claw", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Slashing })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Gore(SizeType sizeType) => 
            WeaponBuilder.Create("Gore", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 6 })
                .SetDamageTypes(new[] { WeaponDamageType.Piercing })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Hoof(SizeType sizeType) => 
            WeaponBuilder.Create("Hoof", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Secondary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Tentacle(SizeType sizeType) => 
            WeaponBuilder.Create("Tentacle", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Secondary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Wing(SizeType sizeType) => 
            WeaponBuilder.Create("Wing", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Secondary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Pincer(SizeType sizeType) => 
            WeaponBuilder.Create("Pincer", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 6 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Secondary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon TailSlap(SizeType sizeType) => 
            WeaponBuilder.Create("Tail Slap", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 6 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Secondary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Slam(SizeType sizeType) => 
            WeaponBuilder.Create("Slam", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Bludgeoning })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Sting(SizeType sizeType) => 
            WeaponBuilder.Create("Sting", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Piercing })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();

        public static Weapon Talons(SizeType sizeType) => 
            WeaponBuilder.Create("Talons", WeaponProficiencyType.Simple, WeaponEffortType.Unarmed, new[] { 1, 4 })
                .SetDamageTypes(new[] { WeaponDamageType.Slashing })
                .SetWeaponAttackType(WeaponAttackType.Primary)
                .SetSizeType(sizeType)
                .SetCost(0)
                .SetWeight(0)
                .SetDescription("")
                .Build();
    }

    public class Weapon : BaseItem
    {
        private SizeType _weaponSizeType;
        public SizeType WeaponSizeType
        {
            get => _weaponSizeType;
            set
            {
                DamageRoll = value == SizeType.Medium ? MediumDamageRoll : WeaponDamageSizeConversion(value, MediumDamageRoll);
                _weaponSizeType = value;
            }
        }

        public WeaponProficiencyType WeaponProficiencyType { get; }
        public WeaponEffortType WeaponEffortType { get; }
        public WeaponAttackType WeaponAttackType { get; }

        public int[] DamageRoll { get; private set; }

        public int[] MediumDamageRoll { get; }
        public int CriticalMinRoll { get; }
        public int CriticalMultiplier { get; }
        public WeaponDamageType[] WeaponDamageTypes { get; }
        public int Range { get; }

        public int MinDmg => DamageRoll[0] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);
        public int MaxDmg => DamageRoll[0] * DamageRoll[1] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);

        public bool IsCriticalRoll(int roll) => roll >= CriticalMinRoll;

        public Weapon(string name, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] mediumDamageRoll, WeaponAttackType weaponAttackType, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, SizeType sizeType, double cost, double weight, string description) : base(name, cost, weight, description)
        {
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            MediumDamageRoll = mediumDamageRoll;
            WeaponAttackType = WeaponAttackType;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;
            WeaponSizeType = sizeType;
        }

        public static Dictionary<string, List<int[]>> MediumDamageSizeConversionDict = new Dictionary<string, List<int[]>>
        {
            {"1d2",  new List<int[]> {new[] {0, 0},new[] {1, 1},new[] {1, 2},new[] {1, 3},new[] {1, 4}}},
            {"1d3",  new List<int[]> {new[] {1, 1},new[] {1, 2},new[] {1, 3},new[] {1, 4},new[] {1, 6}}},
            {"1d4",  new List<int[]> {new[] {1, 2},new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {2, 8}}},
            {"1d6",  new List<int[]> {new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {2, 8},new[] {4, 6}}},
            {"1d8",  new List<int[]> {new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {3, 6}}},
            {"1d10", new List<int[]> {new[] {1, 6},new[] {1, 8},new[] {1,10},new[] {2, 8},new[] {3, 8}}},
            {"1d12", new List<int[]> {new[] {1, 8},new[] {1,10},new[] {1,12},new[] {3, 6},new[] {4, 6}}},
            {"2d4",  new List<int[]> {new[] {1, 4},new[] {1, 6},new[] {2, 4},new[] {2, 6},new[] {3, 6}}},
            {"2d6",  new List<int[]> {new[] {1, 8},new[] {1,10},new[] {2, 6},new[] {3, 6},new[] {4, 6}}},
            {"2d8",  new List<int[]> {new[] {1,10},new[] {2, 6},new[] {2, 8},new[] {3, 8},new[] {4, 8}}},
            {"2d10", new List<int[]> {new[] {2, 6},new[] {2, 8},new[] {2,10},new[] {4, 8},new[] {6, 8}}},
        };

        public static int[] WeaponDamageSizeConversion(SizeType sizeType, int[] currentDamage)
        {
            var key = $"{currentDamage[0]}d{currentDamage[1]}";
            if (!MediumDamageSizeConversionDict.TryGetValue(key, out var list))
            {
                throw new Exception($"Unknown key '{key}', for MediumDamageSizeConversionDict!");
            }

            if (sizeType == SizeType.Diminutive || sizeType == SizeType.Fine || (sizeType == SizeType.Colossal ||
                sizeType == SizeType.Gargantuan) && list.Count < 7)
            {
                throw new Exception($"Not supported sizetype for weapon damage conversion '{sizeType}'.");
            }
            return list[(int)sizeType - 2];
        }

    }
}