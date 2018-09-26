using GoRogue;

namespace WoMFramework.Game.Model.Dungeon
{
    /// <summary>
    /// Represents the basic square tile.
    /// </summary>
    public abstract class Tile
    {
        // for algorithms
        private int _d;
        private Tile _p;

        public readonly Room Parent;
        public readonly Coord Coordinate;
        public readonly Wall[] Sides = new Wall[4];


        public int Height;

        protected Tile(Room parent, Coord coordinate)
        {
            Parent = parent;
            Coordinate = coordinate;
        }

        public abstract bool IsReachable { get; }

        public abstract bool IsSolid { get; }

        //public abstract void Interact(Mogwai mog);

        public bool IsOccupied { get; set; }

        public Wall GetSide(Direction direction)
        {
            return Sides[(int)direction];
        }


        public override string ToString()
        {
            return $"[{Coordinate}]";
        }
    }

    public class StoneTile : Tile
    {
        public override bool IsSolid => true;
        public override bool IsReachable => !IsOccupied;

        public StoneTile(Room parent, Coord coordinate) : base(parent, coordinate)
        {
        }
    }

    public class WaterTile : Tile
    {
        public override bool IsSolid => false;
        public override bool IsReachable => false;

        public WaterTile(Room parent, Coord coordinate) : base(parent, coordinate)
        {
        }
    }
}
