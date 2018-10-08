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
