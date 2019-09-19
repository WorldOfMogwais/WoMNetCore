using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
    public class AdventureEntityContainer
    {
        public List<AdventureEntity> Entities { get; }

        public bool IsPassable => Entities.All(p => p.IsPassable);

        public AdventureEntityContainer()
        {
            Entities = new List<AdventureEntity>();
        }

        public void Add(AdventureEntity entitiy)
        {
            Entities.Add(entitiy);
        }

        public void Remove(AdventureEntity entitiy)
        {
            Entities.Remove(entitiy);
        }

        public bool Has<T>()
        {
            return Entities.Any(p => p is T);
        }

        public IEnumerable<T> Get<T>()
        {
            return Entities.OfType<T>();
        }

    }

    public abstract class AdventureEntity : SpellEnabled
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

        public bool IsPassable { get; set; }

        public int AdventureEntityId { get; set; }

        public int Size { get; }

        public bool HasLoot => IsLootable && LootState != LootState.None && LootState != LootState.Looted ? true : false;

        public bool IsLootable { get; }

        public LootState LootState { get; set; }

        public Treasure Treasure { get; set; }

        public virtual bool TakeAction(EntityAction entityAction)
        {
            return false;
        }
    }
}