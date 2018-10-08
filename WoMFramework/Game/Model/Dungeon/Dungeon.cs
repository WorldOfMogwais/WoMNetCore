using System;
using GoRogue;
using WoMFramework.Game.Combat;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model.Dungeon
{
    public class Dungeon : Adventure
    {
        public readonly Shift CreationShift;
        public readonly Dice DungeonDice;

        public int Level { get; protected set; }

        public override Map Map { get; set; }

        //public bool[,] Blueprint { get; protected set; }

        public Dungeon(Shift creationShift)
        {
            CreationShift = creationShift;
            DungeonDice = creationShift.MogwaiDice; // set dungeon dice using the creation shift
        }

        public override void NextStep(Mogwai.Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                Initialise();
                AdventureState = AdventureState.Running;
            }

            if (!Enter(mogwai))
            {
                AdventureState = AdventureState.Failed;
                return;
            }

            AdventureState = AdventureState.Completed;
        }

        public virtual void Initialise()
        {

        }

        public virtual bool Enter(Mogwai.Mogwai mogwai)
        {
            return false;
        }
        
        public void Print()
        {

        }
    }

    /// <summary>
    /// Dungeon with one monster room.
    /// Should be removed later.
    /// </summary>
    public class SimpleDungeon : Dungeon
    {
        protected Monster.Monster[] _monsters;

        public SimpleDungeon(Shift shift) : base(shift)
        {
            Map = new Map(36, 13, this);
        }


        public override void Initialise()
        {

            // deploy monsters and the adventurer
            Monster.Monster[] monsters = { Monsters.Rat };
            for (var i = 0; i < monsters.Length; i++)
            {
                monsters[i].Dice = DungeonDice;
                // TODO: Positioning monsters
                //var monsterCoord = Coord.Get(Map.Width - 2, Map.Height - 2);
                Coord monsterCoord = Coord.Get(0, 0);
                for (int x = Map.Width - 1; x >= 0; x--)
                {
                    bool found = false;
                    for (int y = Map.Height - 1; y >= 0; y--)
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

                Map.AddEntity(monsters[i], monsterCoord);
            }

            _monsters = monsters;
        }

        public override bool Enter(Mogwai.Mogwai mogwai)
        {
            // TODO: Mogwais' initial coordinate should be the entering door's location.
            //var mogCoord = Coord.Get(Width / 2, 1);
            Coord mogCoord = Coord.Get(0, 0);
            for (int x = 0; x < Map.Width; x++)
            {
                bool found = false;
                for (int y = 0; y < Map.Height; y++)
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

            return false;
        }
    }
}