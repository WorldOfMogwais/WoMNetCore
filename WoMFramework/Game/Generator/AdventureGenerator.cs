using System;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;
using WoMFramework.Game.Combat;
using System.Collections.Generic;

namespace WoMFramework.Game.Generator
{
    public class AdventureGenerator
    {
        public static Adventure Create(Shift generatorShift, AdventureAction adventureAction)
        {
            switch (adventureAction.AdventureType)
            {
                case Enums.AdventureType.TEST_ROOM:
                    //return CreateTestRoom(adventureAction.ChallengeRating);
                default:
                    throw new NotImplementedException();
            }
        }

        //private static TestRoom CreateTestRoom(int challengeRatingt)
        //{
        //    SimpleCombat simpleFight = new SimpleCombat(new SimpleRoom(null, null), new List<Monster> {Monsters.Rat, Monsters.Rat});
        //    TestRoom testRoom = new TestRoom(simpleFight);
        //    return testRoom;
        //}
    }
}
