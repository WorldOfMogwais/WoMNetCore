using GoRogue.MapViews;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Troschuetz.Random.Generators;
using WoMFramework.Tool;

namespace WoMConsole
{
    class Program
    {
        private static StandardGenerator random = new StandardGenerator();

        private static int width = 5;
        private static int height = 5;
        private static int tries = 100;

        static void Main(string[] args)
        {
            ConcurrentDictionary<string, double> probOfMaps = new ConcurrentDictionary<string, double>();

            Console.WriteLine("Press ESC to stop");



            do
            {
                while (!Console.KeyAvailable)
                {

                    Parallel.Invoke(
                        () => { var p1 = GetPropOfMap(1); probOfMaps.TryAdd(p1.Key, p1.Value); },
                        () => { var p1 = GetPropOfMap(2); probOfMaps.TryAdd(p1.Key, p1.Value); },
                        () => { var p1 = GetPropOfMap(3); probOfMaps.TryAdd(p1.Key, p1.Value); },
                        () => { var p1 = GetPropOfMap(4); probOfMaps.TryAdd(p1.Key, p1.Value); },
                        () => { var p1 = GetPropOfMap(5); probOfMaps.TryAdd(p1.Key, p1.Value); }
                    ); //close parallel.invoke

                    Console.WriteLine($"{probOfMaps.Count / Math.Pow(2, width * height)} %");
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

            var sortedProbOfMaps = probOfMaps.ToList();

            sortedProbOfMaps.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            sortedProbOfMaps.ForEach(p => Console.WriteLine($"{p.Key} -> {p.Value}"));
        }

        public static KeyValuePair<string, double> GetPropOfMap(int seed)
        {
            var pattern = RandomArray(seed, width, height);
            string patternName = ArrayMapHelper.EncodeBase64(pattern);
            var result = ArrayMapHelper.GetStats(100, 100, tries, pattern);
            return new KeyValuePair<string, double>(patternName, result.Average());
        }

        static ArrayMap<bool> RandomArray(int seed, int width, int height)
        {
            random.Seed = (uint)seed;
            var result = new ArrayMap<bool>(width, height);
            foreach (var pos in result.Positions())
            {
                result[pos] = random.NextBoolean();
            }
            return result;
        }
    }
}
