namespace WoMFramework.Game.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Enums;

    public abstract class SpellEnabled
    {
        protected SpellEnabled()
        {
            foreach (ModifierType modifierType in Enum.GetValues(typeof(ModifierType)))
            {
                TempModifiers.Add(modifierType, new List<Func<Entity, int>> { e => 0 });
                MiscModifiers.Add(modifierType, new List<Func<Entity, int>> { e => 0 });
            }
        }

        public int AccumulateMiscModifiers(Entity e, ModifierType modifierType) => MiscModifiers[modifierType].Sum(t => t.Invoke(e));
        public int AccumulateTempModifiers(Entity e, ModifierType modifierType) => TempModifiers[modifierType].Sum(t => t.Invoke(e));

        public Dictionary<ModifierType, List<Func<Entity, int>>> MiscModifiers { get; } =
            new Dictionary<ModifierType, List<Func<Entity, int>>>();
        public Dictionary<ModifierType, List<Func<Entity, int>>> TempModifiers { get; } =
            new Dictionary<ModifierType, List<Func<Entity, int>>>();

    }
}
