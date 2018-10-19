using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model.Mogwai
{
    public sealed class Mogwai : Entity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static GameLog History => _currentShift.History;

        private static Shift _currentShift;

        public Shift CurrentShift => Shifts[Pointer];

        public bool CanEvolve => Shifts.ContainsKey(Pointer + 1);

        public Shift PeekNextShift => CanEvolve ? Shifts[Pointer + 1] : null;

        public Dictionary<double, Shift> Shifts { get; }

        public MogwaiState MogwaiState { get; set; }

        public List<int> LevelShifts { get; } = new List<int>();

        public int Pointer { get; private set; }

        public string Key { get; }

        public Coat Coat { get; }

        public Body Body { get; }

        public Stats Stats { get; }

        public Experience Experience { get; set; }

        public double Exp { get; private set; }

        public int CurrentLevel { get; private set; } = 1;

        public double XpToLevelUp => LevelUpXp(CurrentLevel);

        public double LevelUpXp(int level) => level * 1000;

        //public Adventure Adventure { get; set; }

        /// <inheritdoc />
        public override Dice Dice => _currentShift.MogwaiDice;

        public double Rating => (double)(Strength * 3 + Dexterity * 2 + Constitution * 2 + Inteligence * 3 + Wisdom + Charisma) / 12;

        public Mogwai(string key, Dictionary<double, Shift> shifts)
        {
            Key = key;
            Shifts = shifts;

            _currentShift = shifts.Values.First();

            LevelShifts.Add(_currentShift.Height); // adding initial creation level up

            Pointer = _currentShift.Height;

            // create appearance           
            var hexValue = new HexValue(_currentShift);
            Name = NameGen.GenerateName(hexValue);
            Body = new Body(hexValue);
            Coat = new Coat(hexValue);
            Stats = new Stats(hexValue);

            // create abilities
            var rollEvent = new[] { 4, 6, 3 };
            Gender = _currentShift.MogwaiDice.Roll(2, -1);
            Strength = _currentShift.MogwaiDice.Roll(rollEvent);
            Dexterity = _currentShift.MogwaiDice.Roll(rollEvent);
            Constitution = _currentShift.MogwaiDice.Roll(rollEvent);
            Inteligence = _currentShift.MogwaiDice.Roll(rollEvent);
            Wisdom = _currentShift.MogwaiDice.Roll(rollEvent);
            Charisma = _currentShift.MogwaiDice.Roll(rollEvent);

            BaseSpeed = 30;

            NaturalArmor = 0;
            SizeType = SizeType.Medium;

            BaseAttackBonus = new[] { 0 };

            Faction = Faction.Hero;

            // create experience
            Experience = new Experience(_currentShift);

            // add weaponslot
            Equipment.WeaponSlots.Add(new WeaponSlot());
            // add simple rapier as weapon
            EquipWeapon(Weapons.Instance.ByName("Warhammer"));

            // add simple rapier as weapon
            Equipment.Armor = Armors.StuddedLeather;

            // create slot types
            Equipment.CreateEquipmentSlots(new SlotType[] 
                {SlotType.Head, SlotType.Shoulders, SlotType.Neck,
                 SlotType.Chest,SlotType.Body, SlotType.Belt,SlotType.Wrists,
                 SlotType.Hands,SlotType.Ring1,SlotType.Ring2,SlotType.Feet});

            HitPointDice = 6;
            CurrentHitPoints = MaxHitPoints;

            EnvironmentTypes = new[] { EnvironmentType.Any };

        }

        public bool EvolveAdventure()
        {
            // finish un-animated adventures
            if (Adventure != null && Adventure.AdventureState == AdventureState.Running)
            {
                while (Adventure.HasNextFrame())
                {
                    Adventure.NextFrame();
                }

                if (!Adventure.IsActive)
                {
                    Adventure = null;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        public bool Evolve(out GameLog history)
        {        
            // any shift left?
            if (!CanEvolve)
            {
                history = null;
                return false;
            }

            if (EvolveAdventure())
            {
                history = null;
                return true;
            }

            // increase pointer to next block height
            Pointer++;

            // set current shift to the actual shift we process
            _currentShift = Shifts[Pointer];

            //Log.Info(_currentShift.ToString());

            // assign game log for this shift
            history = _currentShift.History;

            // we go for the adventure if there is one up
            if (Adventure != null && Adventure.IsActive)
            {
                history = _currentShift.History;
                Adventure.Enter(this, _currentShift);
                return true;
            }

            Adventure = null;

            // first we always calculated current lazy experience
            var lazyExp = Experience.GetExp(CurrentLevel, _currentShift);
            if (lazyExp > 0)
            {
                AddExp(Experience.GetExp(CurrentLevel, _currentShift));
            }

            if (!_currentShift.IsSmallShift)
            {
                // TODO remove this is only for debug
                Log.Info(_currentShift.ToString());

                switch (_currentShift.Interaction.InteractionType)
                {
                    case InteractionType.Adventure:
                        // finish adventure before starting a new one ...
                        if (Adventure == null || !Adventure.IsActive)
                        {
                            Adventure = AdventureGenerator.Create(_currentShift,
                                (AdventureAction)_currentShift.Interaction);
                            Adventure.Enter(this, _currentShift);
                            return true;
                        }
                        break;

                    case InteractionType.Leveling:
                        var levelingAction = (LevelingAction)_currentShift.Interaction;
                        switch (levelingAction.LevelingType)
                        {
                            case LevelingType.Class:
                                LevelClass(levelingAction.ClassType);
                                break;
                            case LevelingType.Ability:
                                break;
                            case LevelingType.None:
                                break;
                        }
                        break;

                    case InteractionType.Special:
                        var specialAction = (SpecialAction)_currentShift.Interaction;
                        switch (specialAction.SpecialType)
                        {
                            case SpecialType.Heal:
                                if (IsInjured || IsDisabled)
                                {
                                    Heal(int.MaxValue, HealType.DivineHeal);
                                }
                                return true;
                            case SpecialType.Reviving:
                                // TODO check if costtype is correct otherwise prune action
                                if (IsDying || IsDead)
                                {
                                    Heal(CurrentHitPoints > 0 ? 0 : Math.Abs(CurrentHitPoints), HealType.DivineRevive);
                                }
                                return true;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                }
            }

            // lazy health regeneration, only rest healing if he is not dieing TODO check MogwaiState?
            if (MogwaiState == MogwaiState.None && !IsDying)
            {
                Heal(_currentShift.IsSmallShift ? 2 * CurrentLevel : CurrentLevel, HealType.Rest);
            }

            History.Add(LogType.Info, $"Evolved {Coloring.Name(Name)} shift {Coloring.Exp(Pointer)}!");

            // no more shifts to proccess, no more logging possible to the game log
            _currentShift = null;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public bool CanLevelClass(out int levels)
        {
            levels = LevelShifts.Count - (Classes.Count == 0 ? 0 : Classes.Sum(p => p.ClassLevel));

            return levels > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        private void LevelClass(ClassType classType)
        {
            if (!CanLevelClass(out _))
            {
                Log.Warn("Not allowed class leveling action.");
                return;
            }

            var classes = Classes.FirstOrDefault(p => p.ClassType == classType);
            if (Classes.Count == 0 || classes == null)
            {
                Classes.Insert(0, Model.Classes.Classes.GetClasses(classType));
            }
            else if (Classes.Remove(classes))
            {
                Classes.Insert(0, classes);
            }

            var classesLevels = Classes.Sum(p => p.ClassLevel);

            // do the class level up
            Classes[0].ClassLevelUp();

            var dice = Shifts[LevelShifts[classesLevels]].MogwaiDice;

            // level class now
            LevelClass(dice);

            // initial class level
            if (classesLevels == 0)
            {
                AddGold(dice.Roll(Classes[0].WealthDiceRollEvent));
            }

            History.Add(LogType.Info, Coloring.LevelUp($"You feel the power of the {Classes[0].Name}'s!"));
        }

        /// <inheritdoc />
        public override void AddGold(int gold)
        {
            History.Add(LogType.Info, $"You just found +{Coloring.Gold(gold)} gold!");

            Wealth.Gold += gold;
        }

        /// <inheritdoc />
        public override void AddExp(double exp, Monster.Monster monster = null)
        {
            History.Add(LogType.Info,
                monster == null
                    ? $"You just earned +{Coloring.Exp(exp)} experience!"
                    : $"The {Coloring.Name(monster.Name)} gave you +{Coloring.Exp(exp)}!");

            Exp += exp;

            if (Exp >= XpToLevelUp)
            {
                CurrentLevel += 1;
                LevelShifts.Add(_currentShift.Height);
                LevelUp();
            }
        }

        /// <summary>
        /// Passive level up, includes for example hit point roles.
        /// </summary>
        private void LevelUp()
        {
            History.Add(LogType.Info, Coloring.LevelUp("You're mogwai suddenly feels an ancient power around him."));
            History.Add(LogType.Info, $"{Coloring.LevelUp("Congratulations he just made the")} {Coloring.Green(CurrentLevel.ToString())} {Coloring.LevelUp("th level!")}");

            // leveling up will heal you to max hitpoints
            CurrentHitPoints = MaxHitPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shifts"></param>
        public void UpdateShifts(Dictionary<double, Shift> shifts)
        {
            foreach (var shift in shifts)
            {
                if (!Shifts.ContainsKey(shift.Key))
                {
                    Shifts.Add(shift.Key, shift.Value);
                }
            }
        }
    }
}