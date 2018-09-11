using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model
{
    public class HexValue
    {
        private readonly char[] salt;

        private readonly char[] adHexChar;
        private readonly char[] bkHexChar;
        private readonly char[] txHexChar;

        public List<char[]> Salted => new List<char[]>() { salt, bkHexChar, txHexChar };

        public List<char[]> UnSalted => new List<char[]>() { adHexChar };


        public HexValue(Shift shift)
        {
            string saltgrain = HexHashUtil.ByteArrayToString(BitConverter.GetBytes(shift.Time * Math.Pow(shift.Height, 2)));
            salt = String.Concat(Enumerable.Repeat(saltgrain, (int)(64 / saltgrain.Length) + 1)).ToCharArray();

            adHexChar = shift.AdHex.ToCharArray();
            bkHexChar = shift.BkHex.ToCharArray();
            txHexChar = shift.TxHex.ToCharArray();
        }
    }
}