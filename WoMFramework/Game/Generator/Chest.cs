namespace WoMFramework.Game.Generator
{
    using Enums;
    using Model.Actions;
    using System;

    public class Chest : AdventureEntity
    {
        public override bool TakeAction(EntityAction entityAction)
        {
            throw new NotImplementedException();
        }

        public Chest() : base(false, false, 1, true)
        {
            Name = "Chest";
            LootState = LootState.Unlooted;
        }
    }
}
