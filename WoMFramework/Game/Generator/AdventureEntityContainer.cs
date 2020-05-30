namespace WoMFramework.Game.Generator
{
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
}