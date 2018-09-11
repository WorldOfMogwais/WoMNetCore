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

        private SizeType _sizeType = SizeType.Medium;
        // abilities
        private int _strength = 10;
        private int _dexterity = 10;
        private int _constitution = 10;
        private int _intelligence = 10;
        private int _wisdom = 10;
        private int _charisma = 10;
        // speed
        private int _baseSpeed = 20;
        // armor
        private int _naturalArmor = 0;
        // attack
        private int[] _baseAttackBonus = new int[] { 0 };
        // hitcoints
        private int[] _hitPointDiceRollEvent = new int[] { 1, 6, 0 };
        // equipment
        private Weapon _baseWeapon = NaturalWeapon.Bite(SizeType.Medium);
        private Weapon _primaryWeapon = null;
        private Treasure _treasure = null;
        // description
        private string _description = string.Empty;

        public string Name;
        public double ChallengeRating;
        public MonsterType MonsterType;
        public int Experience;

        public string Description { get; set; }

        private MonsterBuilder(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            this.Name = name;
            this.ChallengeRating = challengeRating;
            this.MonsterType = monsterType;
            this.Experience = experience;
        }

        public static MonsterBuilder Create(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            return new MonsterBuilder(name, challengeRating, monsterType, experience);
        }
        public MonsterBuilder SetSizeType(SizeType sizeType)
        {
            this._sizeType = sizeType;
            return this;
        }
        public MonsterBuilder SetAbilities(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
        {
            this._strength = strength;
            this._dexterity = dexterity;
            this._constitution = constitution;
            this._intelligence = intelligence;
            this._wisdom = wisdom;
            this._charisma = charisma;
            return this;
        }
        public MonsterBuilder SetBaseSpeed(int baseSpeed)
        {
            this._baseSpeed = baseSpeed;
            return this;
        }
        public MonsterBuilder SetNaturalArmor(int naturalArmor)
        {
            this._naturalArmor = naturalArmor;
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int baseAttackBonus)
        {
            this._baseAttackBonus = new int[] { baseAttackBonus };
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int[] baseAttackBonus)
        {
            this._baseAttackBonus = baseAttackBonus;
            return this;
        }
        public MonsterBuilder SetHitPointDiceRollEvent(int[] hitPointDiceRollEvent)
        {
            this._hitPointDiceRollEvent = hitPointDiceRollEvent;
            return this;
        }
        public MonsterBuilder SetBaseWeapon(Weapon baseWeapon)
        {
            this._baseWeapon = baseWeapon;
            return this;
        }
        public MonsterBuilder SetPrimaryWeapon(Weapon primaryWeapon)
        {
            this._primaryWeapon = primaryWeapon;
            return this;
        }
        public MonsterBuilder SetTreasure(Treasure treasure)
        {
            this._treasure = treasure;
            return this;
        }
        public MonsterBuilder SetDescription(string description)
        {
            this._description = description;
            return this;
        }
        public Monster Build()
        {

            Monster monster = new Monster(Name, ChallengeRating, MonsterType, Experience)
            {
                SizeType = _sizeType,
                Strength = _strength,
                Dexterity = _dexterity,
                Constitution = _constitution,
                Inteligence = _intelligence,
                Wisdom = _wisdom,
                Charisma = _charisma,
                BaseSpeed = _baseSpeed,
                NaturalArmor = _naturalArmor,
                BaseAttackBonus = _baseAttackBonus,
                HitPointDiceRollEvent = _hitPointDiceRollEvent,
                Treasure = _treasure,
                Description = _description
            };
            monster.Equipment.BaseWeapon = _baseWeapon;
            monster.Equipment.PrimaryWeapon = _primaryWeapon;
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

        public bool RewardedXp { get; set; }

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
