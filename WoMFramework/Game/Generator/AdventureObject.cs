using System;
using System.Collections.Generic;
using System.Text;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
    public class Chest : IAdventureEntity
    {
        public Adventure Adventure { get; set; }

        public Map Map { get; set; }

        public int AdventureEntityId { get; set; }

        public int Size { get; }

        public bool IsStatic => false;

        public bool IsPassable => false;

        public Coord Coordinate { get; set; }

        public bool TakeAction(EntityAction entityAction)
        {
            return false;
        }

        public Treasure Treasure { get; set; }
    }
}
