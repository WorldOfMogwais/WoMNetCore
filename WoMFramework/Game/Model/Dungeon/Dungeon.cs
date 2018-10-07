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

        public Room Entrance { get; protected set; }
        public Room CurrentRoom { get; protected set; }

        //public bool[,] Blueprint { get; protected set; }

        public Dungeon(Shift creationShift)
        {
            CreationShift = creationShift;
            DungeonDice = creationShift.MogwaiDice; // set dungeon dice using the creation shift
            GenerateRooms(creationShift);
        }

        public override void NextStep(Mogwai.Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                Entrance.Initialise();
                AdventureState = AdventureState.Running;
            }

            if (!Enter(mogwai))
            {
                AdventureState = AdventureState.Failed;
                return;
            }

            AdventureState = AdventureState.Completed;
        }

        public bool Enter(Mogwai.Mogwai mogwai)
        {
            CurrentRoom = Entrance;
            return Entrance.Enter(mogwai);
        }
        
        /// <summary>
        /// Generates rooms and corridors
        /// </summary>
        protected virtual void GenerateRooms(Shift shift)
        {

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
        public override Map Map
        {
            get => CurrentRoom.Map;
            set => throw new NotImplementedException();
        }

        public SimpleDungeon(Shift shift) : base(shift)
        {

        }

        protected override void GenerateRooms(Shift shift)
        {
            var n = 1;                      // The size of a room should be determined by information of shift and mogwai
            var blueprint = new bool[n, n]; // This can be substituted with an n*(n - 1) array
            // TODO: create random connected graph from the blueprint.
            // TODO: create a dungeon with long main chain with few side rooms
            // here, it is obviously { { false } }

            // TODO: assign random rooms with probabilities
            // here, the only room is deterministically a monster room
            var rooms = new Room[n];
            for (var i = 0; i < n; i++)
                rooms[i] = new SimpleRoom(this);

            // set entrance (or maybe we can create a specific class for entrance)
            Entrance = rooms[0];

            CurrentRoom = Entrance;
        }
    }

    public class SimpleRoom : MonsterRoom
    {
        private SimpleCombat fight;

        public SimpleRoom(Dungeon parent) : base(parent)
        {
            //const int length = 40;

            Map = new Map(36, 13, parent);
        }

        public override void Initialise()
        {

            // deploy monsters and the adventurer
            Monster.Monster[] monsters = { Monsters.Rat };
            for (var i = 0; i < monsters.Length; i++)
            {
                monsters[i].Dice = Parent.DungeonDice;
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
            
            fight = new SimpleCombat(this, new[] { mogwai }, _monsters);

            return fight.Run();
        }

    }
}