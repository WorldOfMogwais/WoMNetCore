namespace WoMFramework.Game.Model.Mogwai
{
    using Enums;
    using Generator;
    using Home;
    using Interaction;
    using log4net;
    using Random;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public sealed class Mogwai : Entity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static GameLog History => _currentShift.History;

        private static Shift _currentShift;

        public Shift CurrentShift => Shifts[Pointer];

        public bool CanEvolve => Shifts.ContainsKey(Pointer + 1) && !CanEvolveAdventure;

        public bool CanEvolveAdventure => Adventure != null && Adventure.AdventureState == AdventureState.Running;

        public Shift PeekNextShift => CanEvolve ? Shifts[Pointer + 1] : null;

        public ConcurrentDictionary<long, Shift> Shifts { get; }

        public MogwaiState MogwaiState { get; set; }

        public List<int> LevelShifts { get; } = new List<int>();

        public int Pointer { get; private set; }

        public string Key { get; }

        public Coat Coat { get; }

        public Body Body { get; }

        public Stats Stats { get; }

        public Experience Experience { get; set; }

        public double Exp { get; private set; }

        public double XpToLevelUp => LevelUpXp(CurrentLevel);

        public double LevelUpXp(int level) => Math.Pow(level, 2) * 1000;

        //public Adventure Adventure { get; set; }

        /// <inheritdoc />
        public override Dice Dice => _currentShift.MogwaiDice;

        public double Rating => (double)(Strength * 3 + Dexterity * 2 + Constitution * 2 + Intelligence * 3 + Wisdom + Charisma) / 12;

        public HomeTown HomeTown { get; }

        public Mogwai(string key, Dictionary<long, Shift> shifts)
        {
            Key = key;
            Shifts = new ConcurrentDictionary<long, Shift>(shifts);

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
            BaseStrength = _currentShift.MogwaiDice.Roll(rollEvent);
            BaseDexterity = _currentShift.MogwaiDice.Roll(rollEvent);
            BaseConstitution = _currentShift.MogwaiDice.Roll(rollEvent);
            BaseIntelligence = _currentShift.MogwaiDice.Roll(rollEvent);
            BaseWisdom = _currentShift.MogwaiDice.Roll(rollEvent);
            BaseCharisma = _currentShift.MogwaiDice.Roll(rollEvent);

            BaseSpeed = 30;

            NaturalArmor = 0;
            SizeType = SizeType.Medium;

            BaseAttackBonus = new[] { 0 };

            Faction = Faction.Hero;

            // create experience
            Experience = new Experience(_currentShift);

            // create slot types
            Equipment.CreateEquipmentSlots(new[] {
                SlotType.Head, SlotType.Shoulders, SlotType.Neck,
                SlotType.Chest,SlotType.Armor, SlotType.Belt,SlotType.Wrists,
                SlotType.Hands,SlotType.Ring,SlotType.Ring,SlotType.Feet});

            // add weapon slot
            Equipment.WeaponSlots.Add(new WeaponSlot());

            // add simple weapon
            var baseWeapon = Weapons.Instance.ByName("Warhammer");
            AddToInventory(baseWeapon);
            EquipWeapon(baseWeapon);

            // add simple studded leather as armor
            //Equipment.Armor = Armors.StuddedLeather;
            var baseArmor = Armors.Instance.ByName("Studded Leather");
            AddToInventory(baseArmor);
            EquipItem(SlotType.Armor, baseArmor);

            HitPointDice = 6;
            CurrentHitPoints = MaxHitPoints;

            EnvironmentTypes = new[] { EnvironmentType.Any };

            // create home town
            HomeTown = new HomeTown(_currentShift);
        }

        public bool EvolveAdventure()
        {
            // finish un-animated adventures
            if (!CanEvolveAdventure)
            {
                return false;
            }

            while (Adventure.HasNextFrame())
            {
                Adventure.NextFrame();
            }

            Adventure.AdventureLogs.Clear();

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Evolve()
        {
            // any shift left?
            if (!CanEvolve)
            {
                return false;
            }

            // increase pointer to next block height
            Pointer++;

            // set current shift to the actual shift we process
            //_currentShift = Shifts[Pointer];
            if (!Shifts.TryGetValue(Pointer, out _currentShift))
            {
                return false;
            }

            if (Pointer % 180 == 0)
            {
                HomeTown.Shop.Resupply(_currentShift);
            }

            // we go for the adventure if there is one up
            if (Adventure != null && Adventure.CanEnter)
            {
                Adventure.Enter(this, _currentShift);
                return true;
            }

            if (Adventure != null && Adventure.AdventureState == AdventureState.Completed && Adventure.Reward != null)
            {
                AddExp(Adventure.Reward.Exp);
                AddGold(Adventure.Reward.Gold);
            }

            Adventure = null;
            MogwaiState = MogwaiState.HomeTown;

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
                        // only alive mogwais can go to an adventure, finish adventure before starting a new one ...
                        if (CanAct && (Adventure == null || !Adventure.CanEnter))
                        {
                            Adventure = AdventureGenerator.Create(_currentShift,
                                (AdventureAction)_currentShift.Interaction);
                            Adventure.Enter(this, _currentShift);
                            MogwaiState = MogwaiState.Adventure;
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

                        if (SpecialAction(specialAction.SpecialType))
                        {
                            return true;
                        }

                        break;
                }
            }

            // lazy health regeneration, only rest healing if he is not dieing and not dead
            // TODO check MogwaiState?
            if (MogwaiState == MogwaiState.HomeTown && !IsDying && !IsDead)
            {
                Heal(_currentShift.IsSmallShift ? 2 * CurrentLevel : CurrentLevel, HealType.Rest);
            }

            var activity = ActivityLog.Create(ActivityLog.ActivityType.Evolve, ActivityLog.ActivityState.None, new int[] { Pointer }, null);
            History.Add(LogType.Info, activity);

            // no more shifts to process, no more logging possible to the game log
            _currentShift = null;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="specialType"></param>
        /// <returns></returns>
        private bool SpecialAction(SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.Heal:
                    if (IsInjured || IsDisabled)
                    {
                        Heal(int.MaxValue, HealType.DivineHeal);
                        return true;
                    }

                    break;

                case SpecialType.Reviving:
                    // TODO check if costtype is correct otherwise prune action
                    if (IsDying || IsDead)
                    {
                        Heal(CurrentHitPoints > 0 ? 0 : Math.Abs(CurrentHitPoints), HealType.DivineRevive);
                        return true;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
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

            var activity = ActivityLog.Create(ActivityLog.ActivityType.LevelClass, ActivityLog.ActivityState.None, new int[] { (int)classType }, null);
            History.Add(LogType.Info, activity);
            Adventure?.Enqueue(AdventureLog.Info(this, null, activity));
        }

        /// <inheritdoc />
        public override void AddGold(int gold)
        {
            var activity = ActivityLog.Create(ActivityLog.ActivityType.Gold, ActivityLog.ActivityState.None, new int[] { gold }, null);
            History.Add(LogType.Info, activity);
            if (Adventure != null)
            {
                Adventure.Enqueue(AdventureLog.Info(this, null, activity));
                Adventure.AdventureStats[AdventureStats.Gold] = Adventure.AdventureStats[AdventureStats.Gold] + gold;
            }

           
            Wealth.Gold += gold;
        }

        /// <inheritdoc />
        public override void AddExp(double exp, Monster.Monster monster = null)
        {
            var activity = ActivityLog.Create(ActivityLog.ActivityType.Exp, ActivityLog.ActivityState.None, new int[] { (int)exp }, monster);
            History.Add(LogType.Info, activity);
            if (Adventure != null)
            {
                Adventure.Enqueue(AdventureLog.Info(this, null, activity));
                Adventure.AdventureStats[AdventureStats.Experience] = Adventure.AdventureStats[AdventureStats.Experience] + exp;
            }

            Exp += exp;

            while (Exp >= XpToLevelUp)
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
            var activity = ActivityLog.Create(ActivityLog.ActivityType.Level, ActivityLog.ActivityState.None, new int[] { CurrentLevel }, null);
            History.Add(LogType.Info, activity);
            Adventure?.Enqueue(AdventureLog.Info(this, null, activity));

            // level up grant free revive
            SpecialAction(SpecialType.Reviving);

            // level up grant free heal
            SpecialAction(SpecialType.Heal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shifts"></param>
        public void UpdateShifts(Dictionary<long, Shift> shifts)
        {
            foreach (var shift in shifts)
            {
                if (!Shifts.ContainsKey(shift.Key))
                {
                    if (!Shifts.TryAdd(shift.Key, shift.Value))
                    {
                        // leave loop here as we couldn't add
                        break;
                    }
                }
            }
        }
    }
}
