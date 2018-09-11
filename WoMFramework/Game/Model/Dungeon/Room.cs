using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Combat;

namespace WoMFramework.Game.Model
{
    public abstract class Room
    {
        public Dungeon Parent;

        public List<Corridor> OutgoingDoors;

        public List<Corridor> IncomingDoors;

        public Tile[,] Floor { get; protected set; }

        public int Level { get; protected set; }
        public int Width { get; protected set; }
        public int Length { get; protected set; }


        //public Tile[,] Floor { get; protected set; }

        protected readonly bool IsHidden;

        protected readonly bool Trapped;    // TODO

        protected readonly bool Blocked;    // needs door breaching

        // Ideas
        private int layer = 1;              // don't know what to do with this
        private int Illuminance;            // affects initiative?
        // random encounters with special environments (e.g. a barrel full of blackpowder)
        // ornaments, furnitures

        //public RoomType Type { get; private set; }

        protected Room(Dungeon parent)
        {
            Parent = parent;
        }

        public bool TryGetTile(Coordinate coord, out Tile tile)
        {
            if (coord.X >= Width || coord.Y >= Length || coord.X < 0 || coord.Y < 0)
            {
                tile = null;
                return false;
            }

            tile = Floor[coord.X, coord.Y];
            return true;
        }

        public IEnumerable<Tile> GetNeighbours(Coordinate coordinate)
        {
            if (!TryGetTile(coordinate, out Tile tile)) yield break;
            for (int i = 0; i < 4; i++)
                if (TryGetTile(tile.Coordinate.Neighbour((Direction)i), out Tile neighbour))
                    yield return neighbour;
        }

        public abstract bool Enter();

        public virtual void Initialise(Mogwai mogwai)
        {

        }

        // create pointers
        public static void Connect(Room a, Room b)
        {
            var corridor = new Corridor(a, b);
            a.OutgoingDoors.Add(corridor);
            b.IncomingDoors.Add(corridor);
        }
    }

    // not have to be inherited class of Room 
    public class Corridor : Room
    {
        private int[] _corners;     // indices of corner tiles 

        public Room Entrance { get; }
        public Room Exit { get; }

        public Corridor(Room entrance, Room exit) : base(entrance.Parent)
        {
            Entrance = entrance;
            Exit = exit;
        }

        public override bool Enter()
        {
            throw new NotImplementedException();
        }

        public override void Initialise(Mogwai mogwai)
        {
            throw new NotImplementedException();
        }
    }

    public class MonsterRoom : Room
    {
        private readonly List<Monster> _monsters = new List<Monster>();


        public MonsterRoom(Dungeon parent) : base(parent)
        {
        }

        public override void Initialise(Mogwai mogwai)
        {

        }

        public override bool Enter()
        {
            if (Blocked)
            {
                //  need breaching
            }

            if (Trapped)
            {
                // TODO: Trap interaction
            }

            // calculate initiate here maybe?

            return false;
        }

        private void CreateMonsters(Mogwai mogwai)
        {
            // not implemented
            _monsters.Add(Monsters.Rat);

            // dispose the created monsters to floor
        }
    }

    public class SimpleRoom : MonsterRoom
    {
        private readonly SimpleCombat _fight;

        public SimpleRoom(Dungeon parent, Mogwai mogwai) : base(parent)
        {
            const int length = 5;

            Width = length;
            Length = length;
            var floor = new Tile[length, length];

            // Initialise Tiles
            for (int i = 0; i < length; i++)
            for (int j = 0; j < length; j++)
                floor[i, j] = new StoneTile(this, new Coordinate(i, j));


            // deploy monsters and the adventurer
            Monster[] monsters = { Monsters.Rat };
            for (int i = 0; i < monsters.Length; i++)
            {
                while (true)
                {
                    //var x = parent.DungeonDice.Roll(length, -1);
                    //var y = parent.DungeonDice.Roll(length, -1);
                    var x = 4;
                    var y = 4;
                    Tile tile = floor[x, y];
                    if (!tile.IsOccupied)
                    {
                        monsters[i].Coordinate = new Coordinate(x, y);
                        break;
                    }
                }
            }

            mogwai.Coordinate = new Coordinate(length / 2, 0);

            Floor = floor;

            _fight = new SimpleCombat(this, new []{mogwai}, monsters);
            //_fight.Create(mogwai, Parent.CreationShift);
        }


        public override bool Enter()
        {
            return _fight.Run();
        }

    }
}
