namespace WoMFramework.Game.Model.Learnable
{
    using Enums;
    using System;
    using System.Collections.Generic;

    public class SimpleModifier : Modifier
    {
        private Dictionary<ModifierType, Func<Entity, int>> modifierDict;

        public SimpleModifier(ModifierType modifierType, Func<Entity, int> valueFunc)
        {
            modifierDict = new Dictionary<ModifierType, Func<Entity, int>> { { modifierType, valueFunc } };
        }

        public SimpleModifier(Dictionary<ModifierType, Func<Entity, int>> modifier)
        {
            modifierDict = modifier;
        }

        public override Action<Entity> AddMod => e =>
        {
            foreach (var keyValuePair in modifierDict)
            {
                e.MiscModDict()[keyValuePair.Key].Add(keyValuePair.Value);
            }
        };

        public override Action<Entity> RemoveMod => e =>
        {
            foreach (var keyValuePair in modifierDict)
            {
                e.MiscModDict()[keyValuePair.Key].Remove(keyValuePair.Value);
            }
        };

        public static SimpleModifier Get(ModifierType modifierType, Func<Entity, int> valueFunc)
        {
            return new SimpleModifier(modifierType, valueFunc);
        }
    }
}
