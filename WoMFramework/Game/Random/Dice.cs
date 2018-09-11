using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;

namespace WoMFramework.Game.Random
{
    public class Dice
    {
        private int i1 = 0;

        private int i2 = 0;

        private int i3 = 0;

        private string seed1;

        private string seed2;

        private string seed3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shift"></param>
        public Dice(Shift shift)
        {
            string height = shift.Height.ToString();
            height = height.PadLeft(height.Length + height.Length % 2, '0');
            seed1 = HexHashUtil.HashSHA256(shift.AdHex + height);
            seed2 = HexHashUtil.HashSHA256(shift.AdHex + shift.BkHex).Substring(1);
            seed3 = HexHashUtil.HashSHA256(height + shift.BkHex).Substring(3);
        }

        public Dice(Shift shift, int modifier)
        {
            string height = shift.Height.ToString();
            height = height.PadLeft(height.Length + height.Length % 2, 'a');

            string modifierStr = modifier.ToString();
            modifierStr = modifierStr.PadLeft(modifierStr.Length + modifierStr.Length % 2, 'a');

            seed1 = HexHashUtil.HashSHA256(HexHashUtil.HashSHA256(modifierStr) + height);
            seed2 = HexHashUtil.HashSHA256(modifierStr + height).Substring(1);
            seed3 = HexHashUtil.HashSHA256(height + modifierStr + shift.BkHex).Substring(3);
        }

        public int Roll(int diceSides, int modifier)
        {
            return (GetNext() % diceSides) + 1 + modifier;
        }

        public int Roll(int diceSides)
        {
            return (GetNext() % diceSides) + 1;
        }

        public int Roll(DiceType diceType)
        {
            return (GetNext() % GetSides(diceType)) + 1;
        }

        public int Roll(int[] rollEvent) {

            var rolls = new List<int>();
            for (int i = 0; i < rollEvent[0]; i++)
            {
                rolls.Add(Roll(rollEvent[1]));
            }

            // best off
            if (rollEvent.Length > 2 && rollEvent[2] > 0)
            {
                var purgeXlowRolls = rollEvent[0] - rollEvent[2];
                for (int j = 0; purgeXlowRolls > 0 && j  < purgeXlowRolls; j++ )
                {
                    rolls.Remove(rolls.Min());
                }
            }

            return rolls.Sum();
        }

        private int GetNext()
        {
            int s1val = HexHashUtil.GetHexVal(seed1[i1]);
            int s2val = HexHashUtil.GetHexVal(seed2[i2]);
            int s3val = HexHashUtil.GetHexVal(seed3[i3]);
            int value = s1val + s2val + s3val;
            i1 = (i1 + 1) % seed1.Length;
            i2 = i1 == 0 ? (i2 + 1) % seed2.Length : i2;
            i3 = i2 == 0 ? (i3 + 1) % seed3.Length : i3;
            return value;
        }

        private int GetSides(DiceType diceType)
        {
            return int.Parse(diceType.ToString().Substring(1));

        }
    }
}