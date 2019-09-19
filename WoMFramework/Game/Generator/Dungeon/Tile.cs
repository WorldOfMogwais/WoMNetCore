namespace WoMFramework.Game.Generator.Dungeon
{
    using GoRogue;

    /// <summary>
    /// Represents the basic square tile.
    /// </summary>
    public abstract class Tile : AdventureEntity
    {
        // for algorithms
        //private int _d;
        //private Tile _p;

        public int Height;

        protected Tile(Map map, Coord coordinate) : base(false, false, 1, false)
        {
            Adventure = map.Adventure;
            Map = map;
            Coordinate = coordinate;
        }

        public abstract bool IsReachable { get; }

        public abstract bool IsSolid { get; }

        public bool IsOccupied { get; set; }

        public override string ToString()
        {
            return $"[{Coordinate}]";
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
