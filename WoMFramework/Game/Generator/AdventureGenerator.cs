using System;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Interaction;

namespace WoMFramework.Game.Generator
{
    public class AdventureGenerator
    {
        public static Adventure Create(Shift generatorShift, AdventureAction adventureAction)
        {
            switch (adventureAction.AdventureType)
            {
                case AdventureType.TestRoom:
                    return CreateTestRoom(adventureAction.ChallengeRating);
                case AdventureType.Dungeon:
                    return CreateDungeon(generatorShift, adventureAction.ChallengeRating);
                default:
                    throw new NotImplementedException();
            }
        }

        private static SimpleDungeon CreateDungeon(Shift generatorShift, int challengeRating)
        {
            return new SimpleDungeon(generatorShift, challengeRating);
        }

        private static TestRoom CreateTestRoom(int challengeRating)
        {
            var testRoom = new TestRoom();
            return testRoom;
        }
    }
}
