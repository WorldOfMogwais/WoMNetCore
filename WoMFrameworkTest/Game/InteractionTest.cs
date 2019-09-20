namespace WoMFrameworkTest.Game
{
    using WoMFramework.Game.Enums;
    using WoMFramework.Game.Interaction;
    using Xunit;

    [Collection("SystemInteractionFixture")]
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

            var adventure1 = new AdventureAction(AdventureType.TestRoom, DifficultyType.Challenging, 2);
            Assert.Equal(0.01040004m, adventure1.GetValue1());
            Assert.Equal(0.00002002m, adventure1.GetValue2());

            var amount = adventure1.GetValue1();
            var fee = adventure1.GetValue2();

            var adventure2 = (AdventureAction)Interaction.GetInteraction(amount, fee);
            Assert.Equal(0.01040004m, adventure2.GetValue1());
            Assert.Equal(0.00002002m, adventure2.GetValue2());

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

            var shift = new Shift(1,
               1535295740,
               "32f13027e869de56de3c2d5af13f572b67b5e75a18594013ec",
               39741,
               "0000000033dbfc3cc9f3671ba28b41ecab6f547219bb43174cc97bf23269fa88",
               1,
               "db5639553f9727c42f80c22311bd8025608edcfbcfc262c0c2afe9fc3f0bcb29",
               0.01040003m,
               0.00001002m);


            Assert.Equal(InteractionType.Adventure, shift.Interaction.InteractionType);
            Assert.Equal(AdventureType.TestRoom, ((AdventureAction)shift.Interaction).AdventureType);
            Assert.Equal(DifficultyType.Average, ((AdventureAction)shift.Interaction).DifficultyType);
            Assert.Equal(2, ((AdventureAction)shift.Interaction).AveragePartyLevel);
        }
    }
}
