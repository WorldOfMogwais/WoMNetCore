using System;
using System.Collections;
using GoRogue;
using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using GoRogue.MapViews;
using Troschuetz.Random.Generators;

namespace WoMFramework.Tool
{
    public enum RotateType
    {
        DEG90, DEG180, DEG270
    }

    public static class ArrayMapHelper
    {
        public static bool TryGetFirstPatternMatch(ArrayMap<bool> map, ArrayMap<bool> pMap, ArrayMap<bool> pattern, out Coord startCoord)
        {
            startCoord = new Coord();
            foreach (var pos in map.Positions())
            {
                startCoord = pos;

                var match = true;
                foreach (var patPos in pattern.Positions())
                {
                    var xShift = pos.X + patPos.X;
                    var yShift = pos.Y + patPos.Y;
                    if (map.Width <= xShift // check map width limits
                     || map.Height <= yShift // check map height limits
                     || map[xShift, yShift] != pattern[patPos.X, patPos.Y] // check if the pattern matches here with the walk map
                     || pMap[xShift, yShift] // check if the pattern map already has a pattern at this point
                     )
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                {
                    return true;
                }
            }
            return false;
        }

        public static ArrayMap<T> RotateArrayMap<T>(ArrayMap<T> arrayMap, RotateType rotateType)
        {
            ArrayMap<T> newPattern = null;
            switch (rotateType)
            {
                case RotateType.DEG90:
                    newPattern = new ArrayMap<T>(arrayMap.Height, arrayMap.Width);
                    foreach (var pos in arrayMap.Positions())
                    {
                        newPattern[pos.Y, arrayMap.Width - 1 - pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                case RotateType.DEG180:
                    newPattern = new ArrayMap<T>(arrayMap.Width, arrayMap.Height);
                    foreach (var pos in arrayMap.Positions())
                    {
                        newPattern[arrayMap.Width - 1 - pos.Y, arrayMap.Height - 1 - pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                case RotateType.DEG270:
                    newPattern = new ArrayMap<T>(arrayMap.Height, arrayMap.Width);
                    foreach (var pos in arrayMap.Positions())
                    {
                        newPattern[arrayMap.Height - 1 - pos.Y, pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                default:
                    return newPattern;
            }
        }

        public static int[] GetStats(int height, int width, int tries, ArrayMap<bool> pattern)
        {

            StandardGenerator random = new StandardGenerator();

            var wMap = new ArrayMap<bool>(height, width);

            int[] hits = new int[tries];

            for (int i = 0; i < tries; i++)
            {
                QuickGenerators.GenerateCellularAutomataMap(wMap, random);

                var pMap = new ArrayMap<bool>(height, width);

                while (TryGetFirstPatternMatch(wMap, pMap, pattern, out Coord coord))
                {
                    foreach (var pos in pattern.Positions())
                    {
                        pMap[pos.X + coord.X, pos.Y + coord.Y] = true;
                    }

                    hits[i] += 1;
                }

            }

            return hits;

        }

        private static byte[] BitArrayToByteArray(BitArray bits)
        {
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static string EncodeBase64(ArrayMap<bool> tMap, int i = 0)
        {
            bool[] boolArray = new bool[tMap.Width * tMap.Height];
            foreach (var pos in tMap.Positions())
            {
                boolArray[(pos.Y * tMap.Width) + pos.X] = tMap[pos.X, pos.Y];
            }
            BitArray bitArray = new BitArray(boolArray);

            var bits = BitArrayToByteArray(bitArray);

            return $"X{tMap.Width}Y{tMap.Height}N{i.ToString("000")}-{Convert.ToBase64String(bits)}";
        }

        public static ArrayMap<bool> DecodeBase64(string name)
        {
            string[] nameArray = name.Split('-');
            string[] patternTypeArray = nameArray[0]
                .Split('N')[0]
                .Replace("X", "")
                .Split('Y');
            var width = int.Parse(patternTypeArray[0]);
            var height = int.Parse(patternTypeArray[1]);

            //Debug.Log($"{name} X:{width} Y:{height}");

            byte[] array = Convert.FromBase64String(nameArray[1]);

            BitArray bitArray = new BitArray(Convert.FromBase64String(nameArray[1]));

            ArrayMap<bool> tMap = new ArrayMap<bool>(width, height);
            foreach (var pos in tMap.Positions())
            {
                //Debug.Log($"{(pos.Y * tMap.Width) + pos.X}");

                tMap[pos.X, pos.Y] = bitArray[(pos.Y * tMap.Width) + pos.X];
            }

            return tMap;
        }

    }
}
