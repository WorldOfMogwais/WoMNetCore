namespace WoMFramework.Game.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class Modifier
    {
        public abstract Action<Entity> AddMod { get; }
        public abstract Action<Entity> RemoveMod { get; }

        public List<Func<Entity,bool>> RemovalConditions { get; }

        protected Modifier()
        {
            RemovalConditions = new List<Func<Entity, bool>>();
        }

        public virtual bool CheckRemovalConditions(Entity entity)
        {
            return RemovalConditions.Any(p => p.Invoke(entity));
        }

        public virtual void Add(Entity entity)
        {
            AddMod(entity);
        }

        public virtual void Remove(Entity entity)
        {
            RemoveMod(entity);
        }
    }
}
