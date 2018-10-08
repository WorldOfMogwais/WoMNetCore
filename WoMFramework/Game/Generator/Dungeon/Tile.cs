using GoRogue;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator.Dungeon
{
    /// <summary>
    /// Represents the basic square tile.
    /// </summary>
    public abstract class Tile : IAdventureEntity
    {
        // for algorithms
        private int _d;
        private Tile _p;

        public Adventure Adventure { get; set; }
        public Map Map { get; set; }
        public Coord Coordinate { get; set; }


        public int Height;

        protected Tile(Map map, Coord coordinate)
        {
            Adventure = map.Adventure;
            Map = map;
            Coordinate = coordinate;
        }

        public abstract bool IsReachable { get; }

        public abstract bool IsSolid { get; }

        //public abstract void Interact(Mogwai mog);

        public bool IsOccupied { get; set; }

        public override string ToString()
        {
            return $"[{Coordinate}]";
        }


        public bool IsStatic { get; }
        public bool IsPassable { get; }
        public int AdventureEntityId { get; set; }
        public int Size => 1;
        public bool TakeAction(EntityAction entityAction)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StoneTile : Tile
    {
        public override bool IsSolid => true;
        public override bool IsReachable => !IsOccupied;

        public StoneTile(Map map, Coord coordinate) : base(map, coordinate)
        {
        }
    }

    public class StoneWall : Tile
    {
        public override bool IsSolid => true;
        public override bool IsReachable => false;

        public StoneWall(Map map, Coord coordinate) : base(map, coordinate)
        {
        }
    }

    public class WaterTile : Tile
    {
        public override bool IsSolid => false;
        public override bool IsReachable => false;

        public WaterTile(Map map, Coord coordinate) : base(map, coordinate)
        {
        }
    }
}
