using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoMFramework.Game.Model;
using WoMFramework.Tool;

namespace WoMFramework.Game.Random
{
    public class NameGen
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string GenerateName(HexValue hexValue)
        {
            int minLength = GetMinLength(hexValue.UnSalted, 3, 9);
            
            string[] consonants = { "b", "c", "ck", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string name = "";
            double seedValues;
            if (HexHashUtil.TryHexPosConversion(4, 2, hexValue.UnSalted,out seedValues))
            {
                name += consonants[(int)seedValues % consonants.Length];
            }
            if (HexHashUtil.TryHexPosConversion(6, 2, hexValue.UnSalted, out seedValues))
            {
                name += vowels[(int)seedValues % vowels.Length];
            }
            int ind = 8;
            bool consonanNow = true;
            while (name.Length < minLength)
            {
                if (consonanNow && HexHashUtil.TryHexPosConversion(ind, 2, hexValue.UnSalted, out seedValues))
                {
                    name += consonants[(int)seedValues % consonants.Length];
                }
                else if (HexHashUtil.TryHexPosConversion(ind, 2, hexValue.UnSalted, out seedValues))
                {
                    name += vowels[(int)seedValues % vowels.Length];
                }
                else
                {
                    _log.Error("Gnerating names seems currently troublesome!");
                }
                consonanNow = !consonanNow;
                ind += 2;
            }

            return name.First().ToString().ToUpper() + name.Substring(1);
        }

        private static int GetMinLength(List<char[]> unSalted, int minLength, int maxLength)
        {
            int result = 0;
            for(int i = 1; i < 10; i ++)
            {
                if (HexHashUtil.TryHexPosConversion(i, 1, unSalted, out double value))
                {
                    result += (int)value;
                }

                if (result > minLength - 1 & i > 3)
                {
                    if (result < maxLength)
                    {
                        return result;
                    }
                    else
                    {
                        result = maxLength;
                    }
                }
            }

            return maxLength - 1;
        }
    }
}
