namespace WoMFramework.Game.Random
{
    using Enums;
    using Interaction;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Tool;
    using Troschuetz.Random;
    using Troschuetz.Random.Generators;

    public class Dice
    {
        private readonly Shift _shift;

        private int _i1;

        private int _i2;

        private int _i3;

        private readonly string _seed1;

        private readonly string _seed2;

        private readonly string _seed3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shift"></param>
        public Dice(Shift shift)
        {
            _shift = shift;
            var height = shift.Height.ToString();
            height = height.PadLeft(height.Length + height.Length % 2, '0');
            _seed1 = HexHashUtil.HashSha256(shift.AdHex + height);
            _seed2 = HexHashUtil.HashSha256(shift.AdHex + shift.BkHex).Substring(1);
            _seed3 = HexHashUtil.HashSha256(height + shift.BkHex).Substring(3);
        }

        public Dice(Shift shift, int modifier)
        {
            _shift = shift;

            var height = shift.Height.ToString();
            height = height.PadLeft(height.Length + height.Length % 2, 'a');

            var modifierStr = modifier.ToString();
            modifierStr = modifierStr.PadLeft(modifierStr.Length + modifierStr.Length % 2, 'a');

            _seed1 = HexHashUtil.HashSha256(HexHashUtil.HashSha256(modifierStr) + height);
            _seed2 = HexHashUtil.HashSha256(modifierStr + height).Substring(1);
            _seed3 = HexHashUtil.HashSha256(height + modifierStr + shift.BkHex).Substring(3);
        }

        public int Roll(int diceSides, int modifier)
        {
            return GetNext() % diceSides + 1 + modifier;
        }

        public int Roll(int diceSides)
        {
            return GetNext() % diceSides + 1;
        }

        public int Roll(DiceType diceType)
        {
            return (GetNext() % (int) diceType) + 1;
        }

        public int Roll(int[] rollEvent)
        {
            var rolls = new List<int>();
            for (var i = 0; i < rollEvent[0]; i++)
            {
                rolls.Add(Roll(rollEvent[1]));
            }

            // best off
            if (rollEvent.Length > 2 && rollEvent[2] > 0)
            {
                var purgeXlowRolls = rollEvent[0] - rollEvent[2];
                for (var j = 0; purgeXlowRolls > 0 && j < purgeXlowRolls; j++)
                {
                    rolls.Remove(rolls.Min());
                }
            }

            // sum up the rolls
            var result = rolls.Sum();

            // modifier
            if (rollEvent.Length > 3 && rollEvent[3] > 0)
            {
                result += rollEvent[3];
            }

            return result;
        }

        private int GetNext()
        {
            var s1Val = HexHashUtil.GetHexVal(_seed1[_i1]);
            var s2Val = HexHashUtil.GetHexVal(_seed2[_i2]);
            var s3Val = HexHashUtil.GetHexVal(_seed3[_i3]);
            var value = s1Val + s2Val + s3Val;
            _i1 = (_i1 + 1) % _seed1.Length;
            _i2 = _i1 == 0 ? (_i2 + 1) % _seed2.Length : _i2;
            _i3 = _i2 == 0 ? (_i3 + 1) % _seed3.Length : _i3;
            return value;
        }

        public IGenerator GetRandomGenerator()
        {
            var genDice = new Dice(_shift, 1337);
            var seed = 0;
            for (var i = 0; i < 8; i++)
            {
                seed += (int) Math.Pow(10, i) * (genDice.GetNext() % 10);
            }

            return new StandardGenerator(seed);
        }
    }
}
