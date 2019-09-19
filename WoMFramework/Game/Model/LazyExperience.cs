namespace WoMFramework.Game.Model
{
    using Interaction;
    using System;
    using System.Collections.Generic;
    using Tool;

    public class Experience
    {
        private readonly string[] _expPats;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shift"></param>
        public Experience(Shift shift)
        {
            _expPats = GetExpPatterns(shift.TxHex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txHex"></param>
        /// <returns></returns>
        public string[] GetExpPatterns(string txHex)
        {
            var expPatterns = txHex.Replace('0', '8');
            var expPats = new List<string>();
            for (var i = 0; i + 4 <= 44; i = i + 4)
            {
                expPats.Add(expPatterns.Substring(i, 4));
            }

            for (var i = 44; i + 3 <= 62; i = i + 3)
            {
                expPats.Add(expPatterns.Substring(i, 3));
            }

            for (var i = 62; i + 2 <= 64; i = i + 2)
            {
                expPats.Add(expPatterns.Substring(i, 2));
            }

            return expPats.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentLevel"></param>
        /// <param name="shift"></param>
        /// <returns></returns>
        internal double GetExp(int currentLevel, Shift shift)
        {
            var hexSize = shift.BkHex.Length;
            var lazyExpLevel = currentLevel / 10;
            var lazyExp = 0;

            for (var i = 0; i <= lazyExpLevel; i++)
            {
                var exPat = _expPats[i % 18];
                var indExp = shift.BkHex.IndexOf(exPat, StringComparison.Ordinal);
                if (indExp != -1)
                {
                    var charMultiplierA = shift.BkHex[(hexSize + indExp - 1) % hexSize];
                    var charMultiplierB = shift.BkHex[(indExp + exPat.Length) % hexSize];
                    var exp = HexHashUtil.GetHexVal(charMultiplierA) * HexHashUtil.GetHexVal(charMultiplierB);
                    lazyExp += exp;
                }
            }

            return lazyExp;
        }

        internal void Print()
        {
            Console.WriteLine("- Experience:");
            Console.WriteLine(string.Join(";", _expPats));
        }
    }
}
