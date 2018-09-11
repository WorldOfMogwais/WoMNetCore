using WoMFramework.Game.Generator;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Model
{
    public class Dungeon : Adventure
    {
        public readonly Shift CreationShift;
        public readonly Dice DungeonDice;

        public int Level { get; protected set; }

        public Room Entrance { get; protected set; }

        //public bool[,] Blueprint { get; protected set; }

        public Dungeon(Mogwai mogwai, Shift creationShift)
        {
            CreationShift = creationShift;
            DungeonDice = creationShift.MogwaiDice; // set dungeon dice using the creation shift
            GenerateRooms(mogwai, creationShift);

        }

        public override void NextStep(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.CREATION)
            {
                Entrance.Initialise(mogwai);
                AdventureState = AdventureState.RUNNING;
            }

            if (!Enter())
            {
                AdventureState = AdventureState.FAILED;
                return;
            }

            AdventureState = AdventureState.COMPLETED;
        }

        public bool Enter()
        {
            return Entrance.Enter();
        }
        
        /// <summary>
        /// Generates rooms and corridors
        /// </summary>
        protected virtual void GenerateRooms(Mogwai mogwai, Shift shift)
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
        public SimpleDungeon(Mogwai mogwai, Shift shift) : base(mogwai, shift)
        {

        }

        protected override void GenerateRooms(Mogwai mogwai, Shift shift)
        {
            int n = 1;                              // should be determined by information of shift and mogwai

            bool[,] blueprint = new bool[n, n];     // can be substituted with an n*(n - 1) array

            // TODO: create random connected graph from the blueprint.
            // TODO: create a dungeon with long main chain with few side rooms
            // here, it is obviously { { false } }


            // TODO: assign random rooms with probabilities
            // here, the only room is deterministically a monster room
            var rooms = new Room[n];
            for (int i = 0; i < n; i++)
                rooms[i] = new SimpleRoom(this, mogwai);

            // specify pointers
            for (int i = 0; i < n; i++)
            for (int j = i + 1; j < n; j++)    // only concern upper diagonal of the matrix
                if (blueprint[i, j])
                    Room.Connect(rooms[i], rooms[j]);

            // set entrance (or maybe we can create a specific class for entrance)
            Entrance = rooms[0];
        }
    }
}