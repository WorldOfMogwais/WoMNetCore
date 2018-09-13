using System;
using System.Collections.Generic;
using GoRogue;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using WoMFramework.Game.Combat;

namespace WoMFramework.Game.Model
{
    public abstract class Room
    {
        public Dungeon Parent;

        public ArrayMap<bool> WalkabilityMap { get; protected set; }

        public int Width => WalkabilityMap.Width;
        public int Height => WalkabilityMap.Height;

        protected Room(Dungeon parent)
        {
            Parent = parent;
        }

        //public bool TryGetTile(Coordinate coord, out Tile tile)
        //{
        //    if (coord.X >= Width || coord.Y >= Length || coord.X < 0 || coord.Y < 0)
        //    {
        //        tile = null;
        //        return false;
        //    }

        //    tile = Floor[coord.X, coord.Y];
        //    return true;
        //}

        //public IEnumerable<Tile> GetNeighbours(Coordinate coordinate)
        //{
        //    if (!TryGetTile(coordinate, out Tile tile)) yield break;
        //    for (int i = 0; i < 4; i++)
        //        if (TryGetTile(tile.Coordinate.Neighbour((Direction)i), out Tile neighbour))
        //            yield return neighbour;
        //}

        public abstract bool Enter();

        public virtual void Initialise(Mogwai mogwai)
        {

        }

        //// create pointers
        //public static void Connect(Room a, Room b)
        //{
        //    var corridor = new Corridor(a, b);
        //    a.OutgoingDoors.Add(corridor);
        //    b.IncomingDoors.Add(corridor);
        //}
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
            // door breaching, traps etc

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
            const int length = 7;

            var walkabilityMap = new ArrayMap<bool>(length, length);
            RectangleMapGenerator.Generate(walkabilityMap);
            WalkabilityMap = walkabilityMap;

            // deploy monsters and the adventurer
            Monster[] monsters = { Monsters.Rat };
            for (int i = 0; i < monsters.Length; i++)
            {
                // TODO: Positioning monsters
                while (true)
                {
                    Coord monsterCoord = Coord.Get(4, 4);
                    
                    if (WalkabilityMap[monsterCoord])
                    {
                        monsters[i].Coordinate = monsterCoord;
                        break;
                    }
                }
            }

            // TODO: Mogwais' initial coordinate should be the entering door's location.
            Coord mogCoord = Coord.Get(length / 2, 1);
            if (!walkabilityMap[mogCoord]) throw new Exception();

            mogwai.Coordinate = mogCoord;

            _fight = new SimpleCombat(this, new []{mogwai}, monsters);
        }


        public override bool Enter()
        {
            return _fight.Run();
        }

    }
}
