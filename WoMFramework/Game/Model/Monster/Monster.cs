using System;
using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Equipment;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model.Monster
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

        // abilities
        public int _fortitude = 0;
        public int _reflex = 0;
        public int _will = 0;

        // speed
        private int _baseSpeed = 20;
        // armor
        private int _naturalArmor;
        // attack
        private int[] _baseAttackBonus = { 0 };
        // hitcoints
        private int[] _hitPointDiceRollEvent = { 1, 6, 0 };
        // equipment
        private Weapon _baseWeapon = NaturalWeapon.Bite(SizeType.Medium);
        private Weapon _primaryWeapon;
        private readonly List<Weapon> _weaponsList = new List<Weapon>();
        private TreasureType _treasureType;
        // environement
        private EnvironmentType[] _environmentTypes = new EnvironmentType[] {EnvironmentType.Any};
        // description
        private string _description = string.Empty;

        public string Name;
        public double ChallengeRating;
        public MonsterType MonsterType;
        public int Experience;

        public string Description { get; set; }

        private MonsterBuilder(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            Name = name;
            ChallengeRating = challengeRating;
            MonsterType = monsterType;
            Experience = experience;
        }
        public static MonsterBuilder Create(string name, double challengeRating, MonsterType monsterType, int experience)
        {
            return new MonsterBuilder(name, challengeRating, monsterType, experience);
        }
        public MonsterBuilder SetSizeType(SizeType sizeType)
        {
            _sizeType = sizeType;
            return this;
        }
        public MonsterBuilder SetAbilities(int strength, int dexterity, int constitution, int intelligence, int wisdom, int charisma)
        {
            _strength = strength;
            _dexterity = dexterity;
            _constitution = constitution;
            _intelligence = intelligence;
            _wisdom = wisdom;
            _charisma = charisma;
            return this;
        }
        public MonsterBuilder SetSavingThrows(int fortitude, int reflex, int will)
        {
            _fortitude = fortitude;
            _reflex = reflex;
            _will = will;
            return this;
        }
        public MonsterBuilder SetWeapons(List<Weapon> weaponsList)
        {
            _weaponsList.AddRange(weaponsList);
            return this;
        }
        public MonsterBuilder SetBaseSpeed(int baseSpeed)
        {
            _baseSpeed = baseSpeed;
            return this;
        }
        public MonsterBuilder SetNaturalArmor(int naturalArmor)
        {
            _naturalArmor = naturalArmor;
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int baseAttackBonus)
        {
            _baseAttackBonus = new[] { baseAttackBonus };
            return this;
        }
        public MonsterBuilder SetBaseAttackBonus(int[] baseAttackBonus)
        {
            _baseAttackBonus = baseAttackBonus;
            return this;
        }
        public MonsterBuilder SetHitPointDiceRollEvent(int[] hitPointDiceRollEvent)
        {
            _hitPointDiceRollEvent = hitPointDiceRollEvent;
            return this;
        }
        public MonsterBuilder SetBaseWeapon(Weapon baseWeapon)
        {
            _baseWeapon = baseWeapon;
            return this;
        }
        public MonsterBuilder SetPrimaryWeapon(Weapon primaryWeapon)
        {
            _primaryWeapon = primaryWeapon;
            return this;
        }
        public MonsterBuilder SetTreasure(TreasureType treasureType)
        {
            _treasureType = treasureType;
            return this;
        }
        public MonsterBuilder SetEnvironementTypes(EnvironmentType[] environmentTypes)
        {
            _environmentTypes = environmentTypes;
            return this;
        }
        public MonsterBuilder SetDescription(string description)
        {
            _description = description;
            return this;
        }
        public Monster Build()
        {
            var monster = new Monster(Name, ChallengeRating, MonsterType, Experience)
            {
                SizeType = _sizeType,

                Strength = _strength,
                Dexterity = _dexterity,
                Constitution = _constitution,
                Inteligence = _intelligence,
                Wisdom = _wisdom,
                Charisma = _charisma,

                FortitudeBaseSave = _fortitude,
                ReflexBaseSave = _reflex,
                WillBaseSave = _will,

                BaseSpeed = _baseSpeed,
                NaturalArmor = _naturalArmor,
                BaseAttackBonus = _baseAttackBonus,
                HitPointDiceRollEvent = _hitPointDiceRollEvent,
                TreasureType = _treasureType,
                EnvironmentTypes = _environmentTypes,
                Description = _description
            };
            monster.Equipment.BaseWeapon = _baseWeapon;
            monster.Equipment.PrimaryWeapon = _primaryWeapon;
            monster.Equipment.Weapons = _weaponsList;
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
        }

        public void Initialize(Dice dice)
        {
            Dice = dice;
            HitPointDice = Dice.Roll(HitPointDiceRollEvent);
            CurrentHitPoints = MaxHitPoints;
        }

    }
}
