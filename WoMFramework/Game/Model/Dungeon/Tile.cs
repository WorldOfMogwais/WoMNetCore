using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMFramework.Game.Model
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
        public readonly Coordinate Coordinate;
        public readonly Wall[] Sides = new Wall[4];
       

        public int Height;

        protected Tile(Room parent, Coordinate coordinate)
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
            return Sides[(int) direction];
        }

        /// <summary>
        /// Get tiles comprising the shortest route including this tile and excluding the destination tile.
        /// </summary>
        public Tile[] GetShortestPath(Tile destination)
        {
            var floor = Parent.Floor;
            var width = Parent.Width;
            var length = Parent.Length;

            var list = new List<Tile>(width * length);
            for (int i = 0; i < length; i++)
                for (int j = 0; j < width; j++)
                {
                    var tile = floor[i, j];
                    tile._p = null;
                    tile._d = int.MaxValue;
                    list.Add(tile);
                }

            var current = this;
            _d = 0;
            while (list.Count > 0)
            {
                var cd = int.MaxValue;
                current = null;
                foreach (var t in list)
                {
                    if (cd <= t._d) continue;
                    cd = t._d;
                    current = t;
                }

                if (current == null)
                    throw new System.Exception();

                list.Remove(current);

                if (current == destination)
                    break;

                for (int i = 0; i < 4; i++)
                {
                    if (!(current.Sides[i]?.IsBlocked ?? false) &&
                        Parent.TryGetTile(current.Coordinate.Neighbour((Direction) i), out Tile t) /*&& (t.IsReachable || t != destination)*/)
                    {
                        var alt = current._d + 1;
                        if (alt < t._d)
                        {
                            t._d = alt;
                            t._p = current;
                        }
                    }
                }
            }

            var s = new Stack<Tile>();
            current = current._p;
            if (destination._p != null || destination == this)
            {
                while (current != null && current != this)
                {
                    s.Push(current);
                    current = current._p;
                }

                return s.ToArray();
            }

            return new Tile[0];
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

        public StoneTile(Room parent, Coordinate coordinate) : base(parent, coordinate)
        {
        }
    }

    public class WaterTile : Tile
    {
        public override bool IsSolid => false;
        public override bool IsReachable => false;

        public WaterTile(Room parent, Coordinate coordinate) : base(parent, coordinate)
        {
        }
    }
}
