namespace WoMFramework.Game.Model.Learnable
{
    using Enums;
    using System;
    using System.Collections.Generic;

    public class SimpleModifier : Modifier
    {
        public Dictionary<ModifierType, int> Modifier;

        public SimpleModifier(ModifierType modifierType, int value)
        {
            Modifier = new Dictionary<ModifierType, int> {{modifierType, value}};
        }

        public override Action<Entity> AddMod => e =>
        {
            foreach (var keyValuePair in Modifier)
            {
                e.MiscMod[keyValuePair.Key] = e.MiscMod[keyValuePair.Key] + keyValuePair.Value;
            }
        };

        public override Action<Entity> RemoveMod => e =>
        {
            foreach (var keyValuePair in Modifier)
            {
                e.MiscMod[keyValuePair.Key] = e.MiscMod[keyValuePair.Key] - keyValuePair.Value;
            }
        };

        public static SimpleModifier Get(ModifierType modifierType, int value)
        {
            return new SimpleModifier(modifierType, value);
        }
    }
}
