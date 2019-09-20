namespace WoMFramework.Game.Model.Monster
{
    using Enums;
    using Generator;
    using Random;
    using System.Collections.Generic;

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

        // environment
        public EnvironmentType[] EnvironmentTypes  { get; set; }

        // description
        public string Description { get; set; }

        public Monster Build()
        {
            var monster = new Monster(Name, ChallengeRating, MonsterType, Experience)
            {
                CurrentLevel = (int) ChallengeRating + 1,

                SizeType = SizeType,

                BaseStrength = Strength,
                BaseDexterity = Dexterity,
                BaseConstitution = Constitution,
                BaseIntelligence = Intelligence,
                BaseWisdom = Wisdom,
                BaseCharisma = Charisma,

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

            for (int i = 0; i < WeaponSlots.Count; i++)
            {
                // create new weaponslot, maybe other slots too
                monster.Equipment.WeaponSlots.Add(new WeaponSlot());

                // create item slots if necessary
                //monster.Equipment.CreateEquipmentSlots(new[] {SlotType.??});

                if (WeaponSlots[i].PrimaryWeapon != null)
                {
                    monster.AddToInventory(WeaponSlots[i].PrimaryWeapon);
                }

                if (WeaponSlots[i].SecondaryWeapon != null)
                {
                    monster.AddToInventory(WeaponSlots[i].SecondaryWeapon);
                }

                // add weapon to empty weaponslot
                monster.EquipWeapon(WeaponSlots[i].PrimaryWeapon, WeaponSlots[i].SecondaryWeapon, i);
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

            Treasure = Treasure.Create(dice, TreasureType);
        }
    }
}
