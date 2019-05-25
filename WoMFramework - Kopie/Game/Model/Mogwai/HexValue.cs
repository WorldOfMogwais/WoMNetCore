using System;
using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Interaction;
using WoMFramework.Tool;

namespace WoMFramework.Game.Model.Mogwai
{
    public class HexValue
    {
        private readonly char[] _salt;

        private readonly char[] _adHexChar;
        private readonly char[] _bkHexChar;
        private readonly char[] _txHexChar;

        public List<char[]> Salted => new List<char[]> { _salt, _bkHexChar, _txHexChar };

        public List<char[]> UnSalted => new List<char[]> { _adHexChar };

        public HexValue(Shift shift)
        {
            var saltgrain = HexHashUtil.ByteArrayToString(BitConverter.GetBytes(shift.Time * Math.Pow(shift.Height, 2)));
            _salt = String.Concat(Enumerable.Repeat(saltgrain, 64 / saltgrain.Length + 1)).ToCharArray();

            _adHexChar = shift.AdHex.ToCharArray();
            _bkHexChar = shift.BkHex.ToCharArray();
            _txHexChar = shift.TxHex.ToCharArray();
        }
    }
}