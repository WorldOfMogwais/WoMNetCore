using Xunit;

using System.Collections.Generic;
using WoMFramework.Game.Interaction;

namespace WoMFramework.Game.Tests
{
    public class DiceTest
    {
        [Fact]
        public void RollDiceSimple()
        {
            var shift = new Shift(0D,
               1530914381,
               "32ad9e02792599dfdb6a9d0bc0b924da23bd96b1b7eb4f0a68",
               7234,
               "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
               2,
               "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
               1.00m,
               0.0001m);

            var dice = new Dice(shift);
            var probabilityDict = new Dictionary<int, int>();

            var n = 1000000;

            for (var i = 0; i < 20 * n; i++)
            {
                var roll = dice.Roll(20);
                if (probabilityDict.TryGetValue(roll, out var count))
                {
                    probabilityDict[roll] = count + 1;
                }
                else
                {
                    probabilityDict[roll] = 1;
                }
            }

            foreach (var keyValue in probabilityDict)
            {
                Assert.True(keyValue.Value > 0.9 * n && keyValue.Value < 1.1 * n);
            }

        }

        [Fact]
        public void RollDiceModifierSimple()
        {
            var shift = new Shift(0D,
               1530914381,
               "32ad9e02792599dfdb6a9d0bc0b924da23bd96b1b7eb4f0a68",
               7234,
               "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
               2,
               "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
               1.00m,
               0.0001m);

            var dice = new Dice(shift, 2);
            var probabilityDict = new Dictionary<int, int>();

            var n = 1000000;

            for (var i = 0; i < 20 * n; i++)
            {
                var roll = dice.Roll(20);
                if (probabilityDict.TryGetValue(roll, out var count))
                {
                    probabilityDict[roll] = count + 1;
                }
                else
                {
                    probabilityDict[roll] = 1;
                }
            }

            foreach (var keyValue in probabilityDict)
            {
                Assert.True(keyValue.Value > 0.9 * n && keyValue.Value < 1.1 * n);
            }

        }

        [Fact]
        public void RollDiceEvent()
        {
            var shift = new Shift(0D,
               1530914381,
               "32ad9e02792599dfdb6a9d0bc0b924da23bd96b1b7eb4f0a68",
               7234,
               "00000000090d6c6b058227bb61ca2915a84998703d4444cc2641e6a0da4ba37e",
               2,
               "163d2e383c77765232be1d9ed5e06749a814de49b4c0a8aebf324c0e9e2fd1cf",
               1.00m,
               0.0001m);

            var dice = new Dice(shift);
            var n = 1000000;
            var rollEvent = new int[] { 4, 6, 3 };
            for (var i = 0; i < 20 * n; i++)
            {
                var roll = dice.Roll(rollEvent);
                Assert.True(roll < 19 && roll > 2);
            }

        }
    }
}
