using Xunit;

using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Interaction.Tests
{
    public class InteractionTest
    {
        [Fact]
        public void InteractionSerialisation()
        {
            //decimal amount = 0.12345678m;
            //decimal fee = 0.00011234m;
            //string parm1 = (amount - fee).ToString("0.00000000").Split('.')[1];
            //string saveParm = fee.ToString("0.00000000").Split('.')[1].Substring(4);
            //string costType = parm1.Substring(0, 2);
            //string actionType = parm1.Substring(2, 2);
            //string addParm = parm1.Substring(4, 4);

            AdventureAction adventure1 = new AdventureAction(AdventureType.TEST_ROOM, DifficultyType.CHALLENGING, 2);
            Assert.Equal(0.01040003m, adventure1.GetValue1());
            Assert.Equal(0.00001002m, adventure1.GetValue2());

            decimal amount = adventure1.GetValue1();
            decimal fee = adventure1.GetValue2();

            AdventureAction adventure2 = (AdventureAction) Interaction.GetInteraction(amount, fee);
            Assert.Equal(0.01040003m, adventure2.GetValue1());
            Assert.Equal(0.00001002m, adventure2.GetValue2());

            //Console.WriteLine($"Value1: {adventure.CostType}");
            //Console.WriteLine($"Value1: {adventure.InteractionType}");
            //Console.WriteLine($"Value1: {adventure.AdventureType}");
            //Console.WriteLine($"Value1: {adventure.ChallengeRating}");
            //Console.WriteLine($"Value1: {adventure.DifficultyType}");
            //Console.WriteLine($"Value1: {adventure.AveragePartyLevel}");
            //Console.WriteLine($"Value1: {adventure.ParamAdd1}");
            //Console.WriteLine($"Value2: {adventure.ParamAdd2}");
        }

        [Fact]
        public void AdventureActionInteraction()
        {
            // Adventure adventure1 = new Adventure(AdventureType.TEST_ROOM, DifficultyType.CHALLENGING, 2);

            Shift shift = new Shift(0D,
               1535295740,
               "32f13027e869de56de3c2d5af13f572b67b5e75a18594013ec",
               39741,
               "0000000033dbfc3cc9f3671ba28b41ecab6f547219bb43174cc97bf23269fa88",
               1,
               "db5639553f9727c42f80c22311bd8025608edcfbcfc262c0c2afe9fc3f0bcb29",
               0.01040003m,
               0.00001002m);


            Assert.Equal(shift.Interaction.InteractionType, InteractionType.ADVENTURE);
            Assert.Equal(((AdventureAction)shift.Interaction).AdventureType, AdventureType.TEST_ROOM);
            Assert.Equal(((AdventureAction)shift.Interaction).DifficultyType, DifficultyType.CHALLENGING);
            Assert.Equal(((AdventureAction)shift.Interaction).AveragePartyLevel, 2);
        }
    }
}
