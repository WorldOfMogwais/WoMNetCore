using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model.Monster
{
    public sealed class MonsterBuilder
    {
        public string Name { get; set; }
        public double ChallengeRating { get; set; }
        public MonsterType MonsterType { get; set; }
        public int Experience { get; set; }

        public SizeType SizeType { get; set; }
        // abilities
        public int Strength  { get; set; }
        public int Dexterity  { get; set; }
        public int Constitution  { get; set; }
        public int Intelligence  { get; set; }
        public int Wisdom  { get; set; }
        public int Charisma  { get; set; }

        // abilities
        public int Fortitude  { get; set; }
        public int Reflex  { get; set; }
        public int Will  { get; set; }

        // speed
        public int BaseSpeed  { get; set; }
        // armor
        public int NaturalArmor { get; set; }
        // attack
        public int[] BaseAttackBonus  { get; set; }
        // hitcoints
        public int[] HitPointDiceRollEvent  { get; set; }
        // equipment
        public List<WeaponSlot> WeaponSlots { get; set; }
        public TreasureType TreasureType { get; set; }
        // environement
        public EnvironmentType[] EnvironmentTypes  { get; set; }
        // description
        public string Description { get; set; }

        public Monster Build()
        {
            var monster = new Monster(Name, ChallengeRating, MonsterType, Experience)
            {
                SizeType = SizeType,

                Strength = Strength,
                Dexterity = Dexterity,
                Constitution = Constitution,
                Inteligence = Intelligence,
                Wisdom = Wisdom,
                Charisma = Charisma,

                FortitudeBaseSave = Fortitude,
                ReflexBaseSave = Reflex,
                WillBaseSave = Will,

                BaseSpeed = BaseSpeed,
                NaturalArmor = NaturalArmor,
                BaseAttackBonus = BaseAttackBonus,
                HitPointDiceRollEvent = HitPointDiceRollEvent,
                TreasureType = TreasureType,
                EnvironmentTypes = EnvironmentTypes,
                Description = Description
            };
            foreach (var weaponSlot in WeaponSlots)
            {
                // create new weaponslot
                monster.Equipment.WeaponSlots.Add(new WeaponSlot());
                // add weapon to empty weaponslot
                monster.EquipWeapon(weaponSlot.PrimaryWeapon, weaponSlot.SecondaryWeapon);
            }
            return monster;
        }
    }

    public class Monster : Entity
    {
        public double ChallengeRating { get; }

        public MonsterType MonsterType { get; }

        public int Experience { get; }

        public TreasureType TreasureType { get; set; }

        public string Description { get; set; }

        public bool IsLooted { get; set; }

        public bool RewardedXp { get; set; }

        public Monster(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            Name = name;
            ChallengeRating = challengeRating;
            MonsterType = monsterType;
            Experience = experience;
            Faction = Faction.Monster;
        }

        public void Initialize(Dice dice)
        {
            Dice = dice;
            HitPointDice = Dice.Roll(HitPointDiceRollEvent);
            CurrentHitPoints = MaxHitPoints;
        }

    }
}
