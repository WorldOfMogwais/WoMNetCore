using System;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
    public abstract class AdventureEntity
    {
        protected AdventureEntity(bool isStatic, bool isPassable, int size, bool isLootable)
        {
            IsStatic = isStatic;
            IsPassable = isPassable;
            Size = size;
            IsLootable = isLootable;
        }

        public string Name { get; set; }

        public Adventure Adventure { get; set; }

        public Map Map { get; set; }

        public Coord Coordinate { get; set; }
 
        public bool IsStatic { get; }

        public bool IsPassable { get; }

        public int AdventureEntityId { get; set; }

        public int Size { get; }

        public bool IsLootable { get; }

        public LootState LootState { get; set; }

        public virtual bool TakeAction(EntityAction entityAction)
        {
            return false;
        }
    }
}