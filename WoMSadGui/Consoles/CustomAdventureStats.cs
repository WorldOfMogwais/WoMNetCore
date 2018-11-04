using System;
using System.Linq;
using Microsoft.Xna.Framework;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    internal class CustomAdventureStats : MogwaiConsole
    {
        private readonly Mogwai _mogwai;
        private Console _statsConsole;

        public CustomAdventureStats(Mogwai mogwai, int width, int height) : base("Adventure Extension", "This is a temp. summary expect more here", width, height)
        {
            _statsConsole = new Console(width, height)  {Position = new Point(0, 0)};
            Children.Add(_statsConsole);
            _mogwai = mogwai;
        }

        public void Update()
        {
            _statsConsole.Clear();
            _statsConsole.Fill(DefaultForeground, new Color(100,0,200,150), null);
            switch (_mogwai.Adventure.AdventureState)
            {
                case AdventureState.Extended:
                    _statsConsole.Print(15, 1, $"Next Block get's your mogwai deeper into the dungeon.");
                    _statsConsole.Print(15, 2, $"Be prepared {Coloring.Name(_mogwai.Name)} for the stage,");
                    _statsConsole.Print(15, 3, $"you killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters so far,");
                    _statsConsole.Print(15, 4, $"but {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsAlive))} monsters still waiting for you.");
                    _statsConsole.Print(15, 5, $"Stay save {Coloring.Name(_mogwai.Name)}. May your soul survive!");
                    break;

                case AdventureState.Completed:
                    _statsConsole.Print(15, 1, $"Gratulations {Coloring.Name(_mogwai.Name)} you're a true hero!");
                    _statsConsole.Print(15, 2, $"You killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters and explored");
                    _statsConsole.Print(15, 3, $"this dungeon, no monster was able to take it up with your power!");
                    _statsConsole.Print(15, 4, $"Your name is history, HURRAY HURRAY {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(15, 5, $"   {Coloring.Green("HURRAY HURRAY")}!");
                    break;

                case AdventureState.Failed:
                    _statsConsole.Print(15, 1, $"You where not able to finish this dungeon, {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(15, 2, $"You killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters,");
                    _statsConsole.Print(15, 3, $"but {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsAlive))} monsters where to much for you.");
                    _statsConsole.Print(15, 4, $"May your soul be free now, {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(15, 5, $"   {Coloring.Red("GAME OVER")}!");
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}