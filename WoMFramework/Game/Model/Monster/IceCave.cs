using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Classes;

namespace WoMFramework.Game.Model.Monster
{
    public class IceCave
    {
        public static MonsterBuilder BunnyRat => new MonsterBuilder()
        {
            Name = "Bunny Rat",
            ChallengeRating = 0.25,
            MonsterType = MonsterType.Animal,
            //MonsterSubType = MonsterSubType.None,
            Experience = 100,
            SizeType = SizeType.Tiny,
            Strength = 2,
            Dexterity = 15,
            Constitution = 11,
            Intelligence = 2,
            Wisdom = 13,
            Charisma = 2,
            Fortitude = 2,
            Reflex = 4,
            Will = 1,
            BaseSpeed = 15,
            NaturalArmor = 0,
            BaseAttackBonus = new int[] { 0 },
            HitPointDiceRollEvent = new int[] { 1, 8, 0, 0 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = NaturalWeapon.Bite(SizeType.Tiny)
                }
            },
            TreasureType = TreasureType.None,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "Fecund and secretive, bunny rats are omnivorous rodents that particularly thrive in urban areas."
        };

        public static MonsterBuilder BearWarrior => new MonsterBuilder()
        {
            Name = "Bear Warrior",
            ChallengeRating = 1.00,
            MonsterType = MonsterType.Animal,
            //MonsterSubType = MonsterSubType.None,
            Experience = 400,
            SizeType = SizeType.Medium,
            Strength = 15,
            Dexterity = 10,
            Constitution = 17,
            Intelligence = 8,
            Wisdom = 11,
            Charisma = 8,
            Fortitude = 5,
            Reflex = 0,
            Will = 0,
            BaseSpeed = 30,
            NaturalArmor = 1,
            BaseAttackBonus = new int[] { 1 },
            HitPointDiceRollEvent = new int[] { 2, 8, 0, 6 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = Weapons.Instance.ByName("Battleaxe"),
                    SecondaryWeapon = NaturalWeapon.Claw(SizeType.Medium)
                }
            },
            TreasureType = TreasureType.Incidental,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "Bear warriors, through a special relationship with bear spirits, literally adopt a bear's strength in the rage of battle, actually transforming into bears while they fight."
        };

        public static MonsterBuilder CrystalGuardian => new MonsterBuilder()
        {
            Name = "Crystal Guardian",
            ChallengeRating = 1.00,
            MonsterType = MonsterType.Elemental,
            //MonsterSubType = MonsterSubType.Earth,
            Experience = 400,
            SizeType = SizeType.Large,
            Strength = 19,
            Dexterity = 12,
            Constitution = 17,
            Intelligence = 6,
            Wisdom = 13,
            Charisma = 14,
            Fortitude = 5,
            Reflex = 1,
            Will = 1,
            BaseSpeed = 30,
            NaturalArmor = 4,
            BaseAttackBonus = new int[] { 1 },
            HitPointDiceRollEvent = new int[] { 2, 8, 0, 6 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = NaturalWeapon.Slam(SizeType.Large)
                }
            },
            TreasureType = TreasureType.Incidental,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "An animated cluster of translucent crystals shaped disturbingly like a gemstone Humanoid."
        };

        public static MonsterBuilder ThreeTailedWolf => new MonsterBuilder()
        {
            Name = "Three Tailed Wolf",
            ChallengeRating = 1.00,
            MonsterType = MonsterType.MagicalBeast,
            //MonsterSubType = MonsterSubType.Earth,
            Experience = 200, // Missing
            SizeType = SizeType.Medium,
            Strength = 12,
            Dexterity = 15,
            Constitution = 15,
            Intelligence = 2,
            Wisdom = 12,
            Charisma = 6,
            Fortitude = 5,
            Reflex = 5,
            Will = 1,
            BaseSpeed = 50,
            NaturalArmor = 0,
            BaseAttackBonus = new int[] { 1 },
            HitPointDiceRollEvent = new int[] { 2, 8, 0, 2 },
            WeaponSlots = new List<WeaponSlot>() {
                new WeaponSlot()
                {
                    PrimaryWeapon = NaturalWeapon.Bite(SizeType.Medium)
                    // SpeziallAbillity: ColdBreath
                    //SecondaryWeapon = NaturalWeapon.ColdBreath(SizeType.Medium)
                }
            },
            TreasureType = TreasureType.None,
            EnvironmentTypes = new EnvironmentType[] { EnvironmentType.Any },
            Description = "This powerful three tailed canine watches its prey with piercing cold blue eyes, darting its tongue across sharp white teeth."
        };

    }
}
