using System;
using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{
    public sealed class WeaponBuilder
    {
        public string Name{ get; set; }
        public WeaponBaseType WeaponBaseType{ get; set; }
        public WeaponProficiencyType WeaponProficiencyType{ get; set; }
        public WeaponEffortType WeaponEffortType{ get; set; }
        public int[] DamageMediumRollEvent{ get; set; }

        public WeaponSubType WeaponSubType { get; set; }
        public WeaponAttackType WeaponAttackType { get; set; }
        public int CriticalMinRoll { get; set; } = 20;
        public int CriticalMultiplier { get; set; } = 2;
        public WeaponDamageType[] WeaponDamageTypes { get; set; }
        public int Range { get; set; }
        public SizeType SizeType { get; set; }
        public double Cost { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }

        public Weapon Build()
        {
            return new Weapon(Name, WeaponBaseType, WeaponSubType, WeaponProficiencyType, WeaponEffortType, DamageMediumRollEvent,  WeaponAttackType, CriticalMinRoll, CriticalMultiplier, WeaponDamageTypes, Range, SizeType, Cost, Weight, Description);
        }
    }

    public class NaturalWeapon
    {
        public static Weapon Bite(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Bite",
                WeaponBaseType = WeaponBaseType.Bite,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 6 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Piercing, WeaponDamageType.Slashing },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Claw(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Claw",
                WeaponBaseType = WeaponBaseType.Claw,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning, WeaponDamageType.Slashing },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Gore(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Gore",
                WeaponBaseType = WeaponBaseType.Gore,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 6 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Piercing },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Hoof(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Hoof",
                WeaponBaseType = WeaponBaseType.Hoof,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Secondary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();
        
        public static Weapon Tentacle(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Tentacle",
                WeaponBaseType = WeaponBaseType.Tentacle,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Secondary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Wing(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Wing",
                WeaponBaseType = WeaponBaseType.Wing,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Secondary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Pincer(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Pincer",
                WeaponBaseType = WeaponBaseType.Pincer,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Secondary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 6 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon TailSlap(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Tail Slap",
                WeaponBaseType = WeaponBaseType.TailSlap,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Secondary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 6 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Slam(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Slam",
                WeaponBaseType = WeaponBaseType.Slam,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Bludgeoning },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Sting(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Sting",
                WeaponBaseType = WeaponBaseType.Sting,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Piercing },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

        public static Weapon Talons(SizeType sizeType) => 
            new WeaponBuilder { 
                Name = "Talons",
                WeaponBaseType = WeaponBaseType.Talons,
                WeaponSubType =  WeaponSubType.None,
                WeaponAttackType = WeaponAttackType.Primary,
                WeaponProficiencyType = WeaponProficiencyType.Simple,
                WeaponEffortType = WeaponEffortType.Unarmed,
                DamageMediumRollEvent = new[] { 1, 4 },
                CriticalMinRoll = 20,
                CriticalMultiplier = 2,
                WeaponDamageTypes = new[] { WeaponDamageType.Slashing },
                SizeType = sizeType,
                Cost = 0,
                Weight = 0,
                Description = string.Empty
            }.Build();

    }

    public class Weapon : BaseItem
    {
        public WeaponBaseType WeaponBaseType { get; }
        public WeaponSubType WeaponSubType { get; }
        public WeaponProficiencyType WeaponProficiencyType { get; }
        public WeaponEffortType WeaponEffortType { get; }
        public WeaponAttackType WeaponAttackType { get; }

        public int[] DamageRoll { get; private set; }

        public int[] MediumDamageRoll { get; }
        public int CriticalMinRoll { get; }
        public int CriticalMultiplier { get; }
        public WeaponDamageType[] WeaponDamageTypes { get; }
        public int Range { get; }

        public SizeType SizeType { get; private set; }

        public int MinDmg => DamageRoll[0] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);
        public int MaxDmg => DamageRoll[0] * DamageRoll[1] + (DamageRoll.Length > 3 ? DamageRoll[3] : 0);

        public bool IsCriticalRoll(int roll) => roll >= CriticalMinRoll;

        public Weapon(string name, WeaponBaseType weaponBaseType, WeaponSubType weaponSubType, WeaponProficiencyType weaponProficiencyType, WeaponEffortType weaponEffortType, int[] mediumDamageRoll, WeaponAttackType weaponAttackType, int criticalMinRoll, int criticalMultiplier, WeaponDamageType[] weaponDamageTypes, int range, SizeType sizeType, double cost, double weight, string description) : base(name, cost, weight, description, slotType:SlotType.Weapon)
        {
            WeaponBaseType = weaponBaseType;
            WeaponSubType = weaponSubType;
            WeaponProficiencyType = weaponProficiencyType;
            WeaponEffortType = weaponEffortType;
            MediumDamageRoll = mediumDamageRoll;
            WeaponAttackType = weaponAttackType;
            CriticalMinRoll = criticalMinRoll;
            CriticalMultiplier = criticalMultiplier;
            WeaponDamageTypes = weaponDamageTypes;
            Range = range;

            SetSize(sizeType);

            // standard attack
            CombatActions.Add(CombatAction.CreateWeaponAttack(null, this, false));

            // full attack
            CombatActions.Add(CombatAction.CreateWeaponAttack(null, this, true));
        }

        public void SetSize(SizeType sizeType)
        {
            DamageRoll = sizeType == SizeType.Medium ? MediumDamageRoll : WeaponDamageSizeConversion(sizeType, MediumDamageRoll);
            SizeType = sizeType;
        }

        public static Dictionary<string, List<int[]>> MediumDamageSizeConversionDict = new Dictionary<string, List<int[]>>
        {
            {"1d2",  new List<int[]> {new[] {0, 0},new[] {0, 0},new[] {1, 1},new[] {1, 2},new[] {1, 3},new[] {1, 4}}},
            {"1d3",  new List<int[]> {new[] {0, 0},new[] {1, 1},new[] {1, 2},new[] {1, 3},new[] {1, 4},new[] {1, 6}}},
            {"1d4",  new List<int[]> {new[] {1, 1},new[] {1, 2},new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {2, 8}}},
            {"1d6",  new List<int[]> {new[] {1, 2},new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {2, 8},new[] {4, 6}}},
            {"1d8",  new List<int[]> {new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {2, 6},new[] {3, 6}}},
            {"1d10", new List<int[]> {new[] {1, 4},new[] {1, 6},new[] {1, 8},new[] {1,10},new[] {2, 8},new[] {3, 8}}},
            {"1d12", new List<int[]> {new[] {1, 6},new[] {1, 8},new[] {1,10},new[] {1,12},new[] {3, 6},new[] {4, 6}}},
            {"2d4",  new List<int[]> {new[] {1, 3},new[] {1, 4},new[] {1, 6},new[] {2, 4},new[] {2, 6},new[] {3, 6}}},
            {"2d6",  new List<int[]> {new[] {1, 6},new[] {1, 8},new[] {1,10},new[] {2, 6},new[] {3, 6},new[] {4, 6}}},
            {"2d8",  new List<int[]> {new[] {1, 8},new[] {1,10},new[] {2, 6},new[] {2, 8},new[] {3, 8},new[] {4, 8}}},
            {"2d10", new List<int[]> {new[] {1,10},new[] {2, 6},new[] {2, 8},new[] {2,10},new[] {4, 8},new[] {6, 8}}},
        };

        public static int[] WeaponDamageSizeConversion(SizeType sizeType, int[] currentDamage)
        {
            var key = $"{currentDamage[0]}d{currentDamage[1]}";
            if (!MediumDamageSizeConversionDict.TryGetValue(key, out var list))
            {
                throw new Exception($"Unknown key '{key}', for MediumDamageSizeConversionDict!");
            }

            if (sizeType == SizeType.Fine || (sizeType == SizeType.Colossal ||
                sizeType == SizeType.Gargantuan) && list.Count < 8)
            {
                throw new Exception($"Not supported sizetype for weapon damage conversion '{sizeType}'.");
            }

            return list[(int)sizeType - 1];
        }

    }
}