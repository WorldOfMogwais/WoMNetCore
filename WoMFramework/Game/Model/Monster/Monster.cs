using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model
{
    public sealed class MonsterBuilder
    {

        private SizeType sizeType = SizeType.MEDIUM;
        // abilities
        private int strength = 10;
        private int dexterity = 10;
        private int constitution = 10;
        private int intelligence = 10;
        private int wisdom = 10;
        private int charisma = 10;
        // speed
        private int baseSpeed = 20;
        // armor
        private int naturalArmor = 0;
        // attack
        private int[] baseAttackBonus = new int[] { 0 };
        // hitcoints
        private int[] hitPointDiceRollEvent = new int[] { 1, 6, 0 };
        // equipment
        private Weapon baseWeapon = NaturalWeapon.Bite(SizeType.MEDIUM);
        private Weapon primaryWeapon = null;
        private Treasure treasure = null;
        // description
        private string description = string.Empty;

        public string name;
        public double challengeRating;
        public MonsterType monsterType;
        public int experience;

        public string Description { get; set; }

        private MonsterBuilder(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            this.name = name;
            this.challengeRating = challengeRating;
            this.monsterType = monsterType;
            this.experience = experience;
        }

        public static MonsterBuilder Create(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            return new MonsterBuilder(name, challengeRating, monsterType, experience);
        }
        public MonsterBuilder SetSizeType(SizeType sizeType)
        {
            this.sizeType = sizeType;
            return this;
        }
        public MonsterBuilder SetAbilities(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
        {
            this.strength = strength;
            this.dexterity = dexterity;
            this.constitution = constitution;
            this.intelligence = intelligence;
            this.wisdom = wisdom;
            this.charisma = charisma;
            return this;
        }
        public MonsterBuilder SetBaseSpeed(int baseSpeed)
        {
            this.baseSpeed = baseSpeed;
            return this;
        }
        public MonsterBuilder SetNaturalArmor(int naturalArmor)
        {
            this.naturalArmor = naturalArmor;
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int baseAttackBonus)
        {
            this.baseAttackBonus = new int[] { baseAttackBonus };
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int[] baseAttackBonus)
        {
            this.baseAttackBonus = baseAttackBonus;
            return this;
        }
        public MonsterBuilder SetHitPointDiceRollEvent(int[] hitPointDiceRollEvent)
        {
            this.hitPointDiceRollEvent = hitPointDiceRollEvent;
            return this;
        }
        public MonsterBuilder SetBaseWeapon(Weapon baseWeapon)
        {
            this.baseWeapon = baseWeapon;
            return this;
        }
        public MonsterBuilder SetPrimaryWeapon(Weapon primaryWeapon)
        {
            this.primaryWeapon = primaryWeapon;
            return this;
        }
        public MonsterBuilder SetTreasure(Treasure treasure)
        {
            this.treasure = treasure;
            return this;
        }
        public MonsterBuilder SetDescription(string description)
        {
            this.description = description;
            return this;
        }
        public Monster Build()
        {

            Monster monster = new Monster(name, challengeRating, monsterType, experience)
            {
                SizeType = sizeType,
                Strength = strength,
                Dexterity = dexterity,
                Constitution = constitution,
                Inteligence = intelligence,
                Wisdom = wisdom,
                Charisma = charisma,
                BaseSpeed = baseSpeed,
                NaturalArmor = naturalArmor,
                BaseAttackBonus = baseAttackBonus,
                HitPointDiceRollEvent = hitPointDiceRollEvent,
                Treasure = treasure,
                Description = description
            };
            monster.Equipment.BaseWeapon = baseWeapon;
            monster.Equipment.PrimaryWeapon = primaryWeapon;
            return monster;
        }

    }
    public class Monster : Entity
    {
        public double ChallengeRating { get; }

        public MonsterType MonsterType { get; }

        public int Experience { get; }

        public Treasure Treasure { get; set; }

        public string Description { get; set; }

        public bool IsLooted { get; set; }

        public bool RewardedXP { get; set; }

        public Monster(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            Name = name;
            ChallengeRating = challengeRating;
            MonsterType = monsterType;
            Experience = experience;
        }

        public void Initialize(Dice dice)
        {
            Dice = dice;
            HitPointDice = Dice.Roll(HitPointDiceRollEvent);
            CurrentHitPoints = MaxHitPoints;
        }

    }
}
