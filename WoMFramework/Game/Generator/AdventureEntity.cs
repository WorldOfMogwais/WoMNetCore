using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
    public class AdventureEntityContainer
    {
        private List<AdventureEntity> _entities = new List<AdventureEntity>();

        public void Add(AdventureEntity entitiy)
        {
            _entities.Add(entitiy);
        }

        public void Remove(AdventureEntity entitiy)
        {
            _entities.Remove(entitiy);
        }

        public bool IsPassable => _entities.All(p => p.IsPassable);

        public bool Has<T>()
        {
            return _entities.Any(p => p is T);
        }

        public IEnumerable<T> Get<T>()
        {
            return _entities.OfType<T>();
        }

        public List<AdventureEntity> GetAll => _entities;
        
    }

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

        public bool IsPassable { get; set; }

        public int AdventureEntityId { get; set; }

        public int Size { get; }

        public bool IsLootable { get; }

        public LootState LootState { get; set; }

        public Treasure Treasure { get; set; }

        public virtual bool TakeAction(EntityAction entityAction)
        {
            return false;
        }
    }
}