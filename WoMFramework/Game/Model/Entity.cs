using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model
{
    public abstract class Entity : IAdventureEntity
    {
        public string Name { get; set; }

        public int Gender { get; set; }
        public string GenderStr => ((GenderType)Gender).ToString();

        public SizeType SizeType { get; set; }

        #region abilities

        public int Strength { get; set; }
        public int StrengthMod => Modifier(Strength);

        public int Dexterity { get; set; }
        public int DexterityMod => Modifier(Dexterity);

        public int Constitution { get; set; }
        public int ConstitutionMod => Modifier(Constitution);

        public int Inteligence { get; set; }
        public int InteligenceMod => Modifier(Inteligence);

        public int Wisdom { get; set; }
        public int WisdomMod => Modifier(Wisdom);

        public int Charisma { get; set; }
        public int CharismaMod => Modifier(Charisma);

        private int Modifier(int ability) => (int)Math.Floor((ability - 10) / 2.0);

        #endregion

        // current position
        public Coord Coordinate { get; set; }

        public int AttackRange => Equipment.PrimaryWeapon.Range / 5 + 1;

        // base speed
        public int BaseSpeed { get; set; }

        // calculate encumbarance and stuff like that ...
        public int Speed => BaseSpeed;

        public int NaturalArmor { get; set; }
        // armorclass = 10 + armor bonus + shield bonus + dex modifier + size modifier + natural armor + deflection + misc modifier
        public int ArmorClass => 10 + Equipment.ArmorBonus + Equipment.ShieldBonus + DexterityMod + (int)SizeType + NaturalArmor;

        // hitpoints
        public int HitPointDice { get; set; }
        public int[] HitPointDiceRollEvent { get; set; }
        public List<int> HitPointLevelRolls { get; }
        public int MaxHitPoints => HitPointDice + HitPointLevelRolls.Sum();
        public int CurrentHitPoints { get; set; }

        // initiative = dex modifier + misc modifier
        public int Initiative => DexterityMod;

        #region saving throws

        //saving throw = basesave + abilitymod + misc modifier + magic modifier + temp modifier
        public int FortitudeBaseSave { get; set; }
        public int Fortitude => FortitudeBaseSave + ConstitutionMod;
        public int ReflexBaseSave { get; set; }
        public int Reflex => ReflexBaseSave + DexterityMod;
        public int WillBaseSave { get; set; }
        public int Will => WillBaseSave + WisdomMod;

        #endregion

        // base attack bonus = class dependent value
        public int[] BaseAttackBonus { get; set; }

        // attackbonus = base attack bonus + strength modifier + size modifier
        public int AttackBonus(int attackIndex) => BaseAttackBonus[attackIndex] + StrengthMod + (int)SizeType;

        // attack roll
        public int[] AttackRolls(Dice dice, int attackIndex, int criticalMinRoll = 21)
        {
            var rolls = new List<int>();
            for (var i = 0; i < 3; i++)
            {
                var lastRoll = dice.Roll(DiceType.D20);
                rolls.Add(lastRoll + AttackBonus(attackIndex));
                if (lastRoll < criticalMinRoll)
                    break;
            }
            return rolls.ToArray();
        }

        // initiative roll
        public int InitiativeRoll(Dice dice) => dice.Roll(DiceType.D20) + Initiative;

        // damage
        public int DamageRoll(Dice dice)
        {
            var damage = dice.Roll(Equipment.PrimaryWeapon.DamageRoll) + (Equipment.PrimaryWeapon.WeaponEffortType == WeaponEffortType.TwoHanded ? (int)Math.Floor(1.5 * StrengthMod) : StrengthMod);
            return damage < 1 ? 1 : damage;
        }

        // injury and death
        public HealthState HealthState
        {
            get
            {
                if (CurrentHitPoints == MaxHitPoints)
                {
                    return HealthState.Healthy;
                }
                if (CurrentHitPoints > 0)
                {
                    return HealthState.Injured;
                }
                if (CurrentHitPoints == 0)
                {
                    return HealthState.Disabled;
                }
                if (CurrentHitPoints > -10)
                {
                    return HealthState.Dying;
                }

                return HealthState.Dead;
            }
        }

        // equipment
        public Equipment.Equipment Equipment { get; }

        // wealth
        public Wealth Wealth { get; set; }

        // dice
        public virtual Dice Dice { get; set; }

        public Classes.Classes CurrentClass => Classes.Count > 0 ? Classes[0] : null;
        public List<Classes.Classes> Classes { get; }
        public int GetClassLevel(ClassType classType)
        {
            var classes = Classes.FirstOrDefault(p => p.ClassType == classType);
            return classes?.ClassLevel ?? 0;
        }

        public EnvironmentType[] EnvironmentTypes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Entity()
        {
            // initialize
            HitPointLevelRolls = new List<int>();
            Equipment = new Equipment.Equipment();
            Wealth = new Wealth();
            Classes = new List<Classes.Classes>();
        }

        public void LevelClass(Dice dice)
        {
            BaseAttackBonus = CalculateBaseAttackBonus(Classes.Sum(p => p.ClassAttackBonus));
            FortitudeBaseSave = Classes.Sum(p => p.FortitudeBaseSave);
            ReflexBaseSave = Classes.Sum(p => p.ReflexBaseSave);
            WillBaseSave = Classes.Sum(p => p.WillBaseSave);
            HitPointDiceRollEvent = Classes[0].HitPointDiceRollEvent;
            HitPointLevelRolls.Add(dice.Roll(HitPointDiceRollEvent));
        }

        private int[] CalculateBaseAttackBonus(int attackBonus)
        {
            var currentBaseAttackBonus = attackBonus;

            var baseAttackBonusList = new List<int> { currentBaseAttackBonus };

            for (var i = currentBaseAttackBonus - 5; i > 0; i = i - 5)
            {
                baseAttackBonusList.Add(i);
            }
            return baseAttackBonusList.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="turn"></param>
        /// <param name="target"></param>
        internal void Attack(int turn, Entity target)
        {
            var weapon = Equipment.PrimaryWeapon;

            // all attacks are calculated
            for (var attackIndex = 0; attackIndex < BaseAttackBonus.Length; attackIndex++)
            {

                var attackRolls = AttackRolls(Dice, attackIndex, weapon.CriticalMinRoll);
                var attack = AttackRoll(attackRolls, target.ArmorClass, out var criticalCounts);

                //string turnStr = $" + ¬g{turn.ToString("00")}§: ";
                var attackStr = criticalCounts > 0 ? "critical" : attack.ToString();
                var attackIndexStr = attackIndex + 1 + (attackIndex == 0 ? "st" : "th");
                var message = $"{Coloring.Name(Name)}({Coloring.Hitpoints(CurrentHitPoints)}) {Coloring.Orange(attackIndexStr)} " +
                    $"attack {Coloring.Name(target.Name)} with {Coloring.DarkName(weapon.Name)} roll {Coloring.Attack(attackStr)}[{Coloring.Armor(target.ArmorClass)}]:";

                if (attack > target.ArmorClass || criticalCounts > 0)
                {

                    var damage = DamageRoll(Dice);
                    var criticalDamage = 0;
                    if (criticalCounts > 0)
                    {
                        for (var i = 0; i < weapon.CriticalMultiplier - 1; i++)
                        {
                            criticalDamage += DamageRoll(Dice);
                        }
                    }
                    var criticalStr = criticalDamage > 0 ? $"(+{Coloring.DoCritDmg(criticalDamage)})" : string.Empty;
                    Mogwai.Mogwai.History.Add(LogType.Comb, $"{message} {Coloring.Green("hit for")} {Coloring.DoDmg(damage)}{criticalStr} {Coloring.Green("damage!")}");
                    target.Damage(damage + criticalDamage, DamageType.Weapon);
                }
                else
                {
                    Mogwai.Mogwai.History.Add(LogType.Comb, $"{message} {Coloring.Red("failed")}!");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attackRolls"></param>
        /// <param name="armorClass"></param>
        /// <param name="criticalCount"></param>
        /// <returns></returns>
        private int AttackRoll(int[] attackRolls, int armorClass, out int criticalCount)
        {
            var attack = attackRolls[attackRolls.Length - 1];
            if (attack > armorClass)
            {
                criticalCount = attackRolls.Length - 1;
                return attack;
            }
            criticalCount = attackRolls.Length > 2 ? attackRolls.Length - 2 : 0;
            return attack;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gold"></param>
        public virtual void AddGold(int gold)
        {
            // nothing here ..
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="monster"></param>
        public virtual void AddExp(double exp, Monster.Monster monster = null)
        {
            // nothing here ..
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="healAmount"></param>
        /// <param name="healType"></param>
        public void Heal(int healAmount, HealType healType)
        {
            var missingHealth = MaxHitPoints - CurrentHitPoints;
            if (missingHealth <= 0 || healAmount <= 0)
            {
                return;
            }

            if (missingHealth < healAmount)
            {
                healAmount = missingHealth;
            }

            Mogwai.Mogwai.History.Add(LogType.Heal, $"{Coloring.Name(Name)} restores {Coloring.GetHeal(healAmount)} HP from {healType.ToString()} healing.");
            CurrentHitPoints += healAmount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="damageAmount"></param>
        /// <param name="damageType"></param>
        public void Damage(int damageAmount, DamageType damageType)
        {
            if (damageAmount <= 0)
            {
                return;
            }

            Mogwai.Mogwai.History.Add(LogType.Damg, $"{Coloring.Name(Name)} suffers {Coloring.GetDmg(damageAmount)} HP from {damageType.ToString()} damage.");
            CurrentHitPoints -= damageAmount;

            if (CurrentHitPoints < 1)
            {
                Mogwai.Mogwai.History.Add(LogType.Damg, $"{Coloring.Name(Name)} got a deadly hit, healthstate is {Coloring.Red(HealthState.ToString())}.");
            }
        }


        #region IAdventureEntity

        public Adventure Adventure { get; set; }
        public Map Map { get; set; }
        int IAdventureEntity.AdventureEntityId { get; set; }
        public int Size { get; }
        bool IAdventureEntity.IsStatic => false;
        bool IAdventureEntity.IsPassable => false;

        void IAdventureEntity.MoveArbitrary()
        {
            Coord destination;
            do
            {
                var roll = Dice.Roll(4, -1);
                destination = Coordinate + Map.Directions[roll];
            } while (!Map.WalkabilityMap[destination]);

            Map.MoveEntity(this, destination);
        }
        #endregion
    }
}
