using System;

namespace WoMFramework.Game.Model.Dungeon
{
    public abstract class Room
    {
        public Dungeon Parent;

        public Map Map { get; set; }

        public int Width => Map.Width;
        public int Height => Map.Height;

        protected Room(Dungeon parent)
        {
            Parent = parent;
        }

        public abstract bool Enter(Mogwai.Mogwai mogwai);

        public abstract void Initialise();
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

        public override bool Enter(Mogwai.Mogwai mogwai)
        {
            throw new NotImplementedException();
        }

        public override void Initialise()
        {
            throw new NotImplementedException();
        }
    }

    public class MonsterRoom : Room
    {
        protected Monster.Monster[] _monsters;


        public MonsterRoom(Dungeon parent) : base(parent)
        {
        }

        public override void Initialise()
        {

        }

        public override bool Enter(Mogwai.Mogwai mogwai)
        { 
            // door breaching, traps etc

            // calculate initiate here maybe?

            return false;
        }
    }
}
