using System;
using System.Collections.Generic;
using System.Text;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Generator
{
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
