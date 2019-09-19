namespace WoMFramework.Game.Generator
{
    using Dungeon;
    using Enums;
    using GoRogue;
    using Model.Actions;
    using System.Collections.Generic;
    using System.Linq;

    public class AdventureEntityContainer
    {
        public List<AdventureEntity> Entities { get; }

        public bool IsPassable => Entities.All(p => p.IsPassable);

        public AdventureEntityContainer()
        {
            Entities = new List<AdventureEntity>();
        }

        public void Add(AdventureEntity entity)
        {
            Entities.Add(entity);
        }

        public void Remove(AdventureEntity entity)
        {
            Entities.Remove(entity);
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
