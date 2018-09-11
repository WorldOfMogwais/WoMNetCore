using System;
using System.Collections.Generic;
using static System.Math;

namespace WoMFramework.Game.Model
{
    public enum Direction
    {
        RIGHT, DOWN, LEFT, UP
    }
    /// <summary>
    /// Simple 2D Cartesian coordinate
    /// </summary>
    public struct Coordinate : IEquatable<Coordinate>
    {
        private static readonly Coordinate[] _directions =
        {
            new Coordinate(1, 0), new Coordinate(0, -1), new Coordinate(-1, 0), new Coordinate(0, 1)
        };
            
        public readonly int X;
        public readonly int Y;

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int Length => (Abs(X) + Abs(Y));

        public Coordinate Neighbour(Direction direction)
        {
            return this + Direction(direction);
        }

        public override bool Equals(object obj)
        {
            return obj is Coordinate && Equals((Coordinate)obj);
        }

        public bool Equals(Coordinate other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static int Distance(Coordinate a, Coordinate b)
        {
            return (a - b).Length;
        }

        public static Coordinate Direction(int direction)
        {
            if (direction < 0 || direction > 3)
                throw new ArgumentOutOfRangeException();

            return _directions[direction];
        }

        public static Coordinate Direction(Direction direction)
        {
            return _directions[(int)direction];
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public static Coordinate operator +(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X + b.X, a.Y + b.Y);
        }

        public static Coordinate operator -(Coordinate a, Coordinate b)
        {
            return new Coordinate(a.X - b.X, a.Y - b.Y);
        }

        public static Coordinate operator *(Coordinate a, int k)
        {
            return new Coordinate(a.X * k, a.Y * k);
        }

        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2)
        {
            return coordinate1.Equals(coordinate2);
        }

        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2)
        {
            return !(coordinate1 == coordinate2);
        }
    }
}