using System.Linq;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Generator.Dungeon
{
    public abstract class Dungeon : Adventure
    {
        protected readonly Shift Shift;

        public override Map Map { get; set; }

        public const int MaxRoundsPerBlock = 0;

        protected Dungeon(Shift shift)
        {
            Shift = shift;
        }

        public override void NextStep(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                Prepare(mogwai, shift);
                AdventureState = AdventureState.Running;
            }

            if (AdventureState == AdventureState.Running && !Run(mogwai))
            {
                AdventureState = AdventureState.Failed;
            }
        }

        public abstract void Prepare(Mogwai mogwai, Shift shift);

        public abstract bool Run(Mogwai mogwai);

    }

    /// <summary>
    /// Dungeon with one monster room.
    /// Should be removed later.
    /// </summary>
    public class SimpleDungeon : Dungeon
    {
        protected Monster[] SimpleMonsters;

        public SimpleDungeon(Shift shift) : base(shift)
        {
            Map = new Map(36, 13, this);
            SimpleMonsters = new[] { Monsters.Rat };

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        /// <param name="shift"></param>
        public override void Prepare(Mogwai mogwai, Shift shift)
        {

            // deploy monster, monster packs
            DeployMonsters(shift);

            // deploy boss monsters
            // deploy treasures
            // deploy traps
            // deploy npc
            // deploy quests


            // deploy mogwai
            DeployMogwai(mogwai);
        }

        /// <summary>
        /// Position monster and give them a unique dice.
        /// </summary>
        /// <param name="shift"></param>
        private void DeployMonsters(Shift shift)
        {
            const int mDeriv = 1000;
            for (var i = 0; i < SimpleMonsters.Length; i++)
            {
                // initialize monster
                SimpleMonsters[i].Initialize(new Dice(shift, mDeriv + i));

                // TODO: Positioning monsters
                var monsterCoord = Coord.Get(0, 0);

                for (var x = Map.Width - 1; x >= 0; x--)
                {
                    var found = false;
                    for (var y = Map.Height - 1; y >= 0; y--)
                    {
                        if (Map.WalkabilityMap[x, y])
                        {
                            monsterCoord = Coord.Get(x, y);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                Map.AddEntity(SimpleMonsters[i], monsterCoord);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        private void DeployMogwai(Mogwai mogwai)
        {
            var mogCoord = Coord.Get(0, 0);
            for (var x = 0; x < Map.Width; x++)
            {
                var found = false;
                for (var y = 0; y < Map.Height; y++)
                {
                    if (Map.WalkabilityMap[x, y])
                    {
                        mogCoord = Coord.Get(x, y);
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            Map.AddEntity(mogwai, mogCoord);
        }

        public override bool Run(Mogwai mogwai)
        {
            for (var round = 0; round < MaxRoundsPerBlock && AdventureState == AdventureState.Running; round++)
            {
                if (mogwai.CombatState == CombatState.Initiation)
                {
                    // initiate begin of a combat
                }

                if (mogwai.CombatState == CombatState.Engaged)
                {
                    CombatRound();
                }
                else
                {
                    ExplorationRound();
                }

                // set adventurestate
                EvaluateAdventureState();
            }
            return true;
        }

        private void ExplorationRound()
        {
            throw new System.NotImplementedException();
        }

        private void CombatRound()
        {
            throw new System.NotImplementedException();
        }

        public override void EvaluateAdventureState()
        {
            var monsters = Map.GetEntities().OfType<Monster>().ToList();
            if (monsters.Count > 0)
            {
                var value = monsters.Count(p => p.HealthState == HealthState.Dead) / monsters.Count;
                AdventureStats[Generator.AdventureStats.Monster] = value;
            }
            else
            {
                AdventureStats[Generator.AdventureStats.Monster] = 1;
            }
            
        }
    }
}