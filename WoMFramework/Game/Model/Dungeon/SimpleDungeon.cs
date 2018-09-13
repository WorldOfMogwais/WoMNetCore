using System;
using System.Collections.Generic;
using System.Text;
using GoRogue;
using GoRogue.MapViews;

namespace WoMFramework.Game.Model.GoRogue
{
    public class SimpleDungeon
    {

    }

    public abstract class Room
    {
        protected ArrayMap<bool> WalkabilityMap;

        public Dungeon Parent { get; }

        protected Room(Dungeon parent)
        {
            Parent = parent;
        }

    }

    public class SimpleRoom : Room
    {
        public SimpleRoom(Dungeon parent) : base(parent)
        {
            WalkabilityMap = new ArrayMap<bool>(5, 5);
        }
    }
}
