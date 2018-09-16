using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model
{
    public class Mogwai : Entity
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static GameLog History => _currentShift.History;

        private readonly int _blockHeight;

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

        public double Exp { get; private set; } = 0;

        public int CurrentLevel { get; private set; } = 1;

        public double XpToLevelUp => CurrentLevel * 1000;

        public Adventure Adventure { get; set; }

        public override Dice Dice => _currentShift.MogwaiDice;

        public double Rating => (double) (Strength * 3 + Dexterity * 2 + Constitution * 2 + Inteligence * 3 + Wisdom + Charisma) / 12;

        public Mogwai(string key, Dictionary<double, Shift> shifts)
        {
            Key = key;
            Shifts = shifts;

            _currentShift = shifts.Values.First();

            LevelShifts.Add(_currentShift.Height); // adding initial creation level up

            _blockHeight = _currentShift.Height;
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

            // create experience
            Experience = new Experience(_currentShift);

            // add simple rapier as weapon
            Equipment.PrimaryWeapon = Weapons.Rapier;

            // add simple rapier as weapon
            Equipment.Armor = Armors.StuddedLeather;

            HitPointDice = 8;
            CurrentHitPoints = MaxHitPoints;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blockHeight"></param>
        public bool Evolve(out GameLog history)
        {
            // any shift left?
            if (!CanEvolve)
            {
                history = null;
                return false;
            }

            // increase pointer to next block height
            Pointer++;

            // set current shift to the actual shift we process
            _currentShift = Shifts[Pointer];

            // assign game log for this shift
            history = _currentShift.History;

            // first we always calculated current lazy experience
            var lazyExp = Experience.GetExp(CurrentLevel, _currentShift);
            if (lazyExp > 0)
            {
                AddExp(Experience.GetExp(CurrentLevel, _currentShift));
            }

            if (!_currentShift.IsSmallShift)
            {
                switch (_currentShift.Interaction.InteractionType)
                {
                    case InteractionType.Adventure:
                        // finish adventure before starting a new one ...
                        if (Adventure == null || !Adventure.IsActive)
                        {
                            Adventure = AdventureGenerator.Create(_currentShift,
                                (AdventureAction) _currentShift.Interaction);
                        }
                        break;

                    case InteractionType.Leveling:
                        Console.WriteLine("Received a leveling action!");
                        break;

                    default:
                        break;
                }
            }

            // we go for the adventure if there is one up
            if (Adventure != null && Adventure.IsActive)
            {
                Adventure.NextStep(this, _currentShift);
                return true;
            }

            Adventure = null;

            // lazy health regeneration
            if (MogwaiState == MogwaiState.None)
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
        /// <param name="exp"></param>
        /// <param name="monster"></param>
        public override void AddExp(double exp, Monster monster = null)
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
                LevelUp(_currentShift);
            }
        }

        /// <summary>
        /// Passive level up, includes for example hit point roles.
        /// </summary>
        /// <param name="shift"></param>
        private void LevelUp(Shift shift)
        {
            History.Add(LogType.Info, Coloring.LevelUp("You're mogwai suddenly feels an ancient power around him."));
            History.Add(LogType.Info, $"{Coloring.LevelUp("Congratulations he just made the")} {Coloring.Green(CurrentLevel.ToString())} {Coloring.LevelUp("th level!")}");

            // hit points roll
            HitPointLevelRolls.Add(shift.MogwaiDice.Roll(HitPointDice));

            // leveling up will heal you to max hitpoints
            CurrentHitPoints = MaxHitPoints;
        }


        public void EnterSimpleDungeon()
        {
            var dungeon = new SimpleDungeon(this, _currentShift);
            Pointer++;
            _currentShift = Shifts[Pointer];

            dungeon.Enter();
        }

        public void Print()
        {
            var shift = Shifts[0];

            Console.WriteLine("*** Mogwai Nascency Transaction ***");
            Console.WriteLine($"- Time: {shift.Time}");
            Console.WriteLine($"- Index: {shift.BkIndex}");
            Console.WriteLine($"- Amount: {shift.Amount}");
            Console.WriteLine($"- Height: {shift.Height}");
            Console.WriteLine($"- AdHex: {shift.AdHex}");
            Console.WriteLine($"- BlHex: {shift.BkHex}");
            Console.WriteLine($"- TxHex: {shift.TxHex}");

            Console.WriteLine();
            Console.WriteLine("*** Mogwai Attributes ***");
            Console.WriteLine("- Body:");
            Body.All.ForEach(p => Console.WriteLine($"{p.Name}: {p.GetValue()} [{p.MinRange}-{p.Creation - 1}] Var:{p.MaxRange}-->{p.Valid}"));
            Console.WriteLine("- Coat:");
            Coat.All.ForEach(p => Console.WriteLine($"{p.Name}: {p.GetValue()} [{p.MinRange}-{p.Creation - 1}] Var:{p.MaxRange}-->{p.Valid}"));
            Console.WriteLine("- Stats:");
            Stats.All.ForEach(p => Console.WriteLine($"{p.Name}: {p.GetValue()} [{p.MinRange}-{p.Creation - 1}] Var:{p.MaxRange}-->{p.Valid}"));
            Experience.Print();
        }

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