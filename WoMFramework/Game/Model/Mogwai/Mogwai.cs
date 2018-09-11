using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model
{
    public class Mogwai : Entity
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static GameLog History => currentShift.History;

        private readonly int blockHeight;

        private static Shift currentShift;

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

        public override Dice Dice => currentShift.MogwaiDice;

        public double Rating => (double) (Strength * 3 + Dexterity * 2 + Constitution * 2 + Inteligence * 3 + Wisdom + Charisma) / 12;

        public Mogwai(string key, Dictionary<double, Shift> shifts)
        {
            Key = key;
            Shifts = shifts;

            currentShift = shifts.Values.First();

            LevelShifts.Add(currentShift.Height); // adding initial creation level up

            blockHeight = currentShift.Height;
            Pointer = currentShift.Height;

            // create appearance           
            var hexValue = new HexValue(currentShift);
            Name = NameGen.GenerateName(hexValue);
            Body = new Body(hexValue);
            Coat = new Coat(hexValue);
            Stats = new Stats(hexValue);

            // create abilities
            int[] rollEvent = new int[] { 4, 6, 3 };
            Gender = currentShift.MogwaiDice.Roll(2, -1);
            Strength = currentShift.MogwaiDice.Roll(rollEvent);
            Dexterity = currentShift.MogwaiDice.Roll(rollEvent);
            Constitution = currentShift.MogwaiDice.Roll(rollEvent);
            Inteligence = currentShift.MogwaiDice.Roll(rollEvent);
            Wisdom = currentShift.MogwaiDice.Roll(rollEvent);
            Charisma = currentShift.MogwaiDice.Roll(rollEvent);

            BaseSpeed = 30;

            NaturalArmor = 0;
            SizeType = SizeType.MEDIUM;

            BaseAttackBonus = new int[] { 0 };

            // create experience
            Experience = new Experience(currentShift);

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
            if (!Shifts.ContainsKey(Pointer + 1))
            {
                history = null;
                return false;
            }

            // increase pointer to next block height
            Pointer++;

            // set current shift to the actual shift we process
            currentShift = Shifts[Pointer];

            // assign game log for this shift
            history = currentShift.History;

            // first we always calculated current lazy experience
            double lazyExp = Experience.GetExp(CurrentLevel, currentShift);
            if (lazyExp > 0)
            {
                AddExp(Experience.GetExp(CurrentLevel, currentShift));
            }

            // we go for the adventure if there is one up
            if (Adventure != null && Adventure.IsActive)
            {
                Adventure.NextStep(this, currentShift);
                return true;
            }

            Adventure = null;


            if (!currentShift.IsSmallShift)
            {
                switch (currentShift.Interaction.InteractionType)
                {
                    case InteractionType.ADVENTURE:
                        Adventure = AdventureGenerator.Create(currentShift, (AdventureAction)currentShift.Interaction);
                        break;
                    case InteractionType.LEVELING:
                        Console.WriteLine("Received a leveling action!");
                        break;
                    default:
                        break;
                }
            }

            // lazy health regeneration
            if (MogwaiState == MogwaiState.NONE)
            {
                Heal(currentShift.IsSmallShift ? 2 * CurrentLevel : CurrentLevel, HealType.REST);
            }

            History.Add(LogType.INFO, $"Evolved {Name} shift ¬G{Pointer}§!¬");

            // no more shifts to proccess, no more logging possible to the game log
            currentShift = null;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="monster"></param>
        public override void AddExp(double exp, Monster monster = null)
        {
            if (monster == null)
            {
                History.Add(LogType.INFO, $"You just earned ¬G+{exp}§ experience!¬");
            }
            else
            {
                History.Add(LogType.INFO, $"The ¬C{monster.Name}§ gave you ¬G+{exp}§!¬");
            }

            Exp += exp;

            if (Exp >= XpToLevelUp)
            {
                CurrentLevel += 1;
                LevelShifts.Add(currentShift.Height);
                LevelUp(currentShift);
            }
        }

        /// <summary>
        /// Passive level up, includes for example hit point roles.
        /// </summary>
        /// <param name="shift"></param>
        private void LevelUp(Shift shift)
        {
            History.Add(LogType.INFO, $"¬YYou're mogwai suddenly feels an ancient power around him.§¬");
            History.Add(LogType.INFO, $"¬YCongratulations he just made the§ ¬G{CurrentLevel}§ ¬Yth level!§¬");

            // hit points roll
            HitPointLevelRolls.Add(shift.MogwaiDice.Roll(HitPointDice));

            // leveling up will heal you to max hitpoints
            CurrentHitPoints = MaxHitPoints;
        }


        public void EnterSimpleDungeon()
        {
            var dungeon = new SimpleDungeon(this, currentShift);
            Pointer++;
            currentShift = Shifts[Pointer];

            dungeon.Enter();
        }

        public void Print()
        {
            Shift shift = Shifts[0];

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

    }
}