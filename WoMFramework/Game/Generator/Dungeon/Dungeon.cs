using GoRogue;
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

            if (!Enter(mogwai))
            {
                AdventureState = AdventureState.Failed;
                return;
            }

            AdventureState = AdventureState.Completed;
        }

        public abstract void Prepare(Mogwai mogwai, Shift shift);

        public abstract bool Enter(Mogwai mogwai);

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

        public override bool Enter(Mogwai mogwai)
        {
            return true;
        }

    }
}