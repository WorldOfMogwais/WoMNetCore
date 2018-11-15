using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    internal class CustomAdventureStats : MogwaiConsole
    {
        private readonly Mogwai _mogwai;

        private Console _base;

        private Console _statsConsole;

        private Console _rewardConsole;

        private Basic _consoleImage;

        public CustomAdventureStats(Mogwai mogwai, int width, int height) : base("Adventure Extension", "This is a temp. summary expect more here", width, height)
        {

            _base = new Console(width, height)  {Position = new Point(0, 0)};
            Children.Add(_base);

            _statsConsole = new Console(width, height)  {Position = new Point(0, 0)};
            Children.Add(_statsConsole);

            _rewardConsole = new Console(27, 8)  {Position = new Point(63, 14)};
            Children.Add(_rewardConsole);

            _mogwai = mogwai;
        }

        public void ChangeImage(string picName)
        {
            _base.Children.Clear();

            // Load the logo
            var imageStream = TitleContainer.OpenStream(picName);
            var image = Texture2D.FromStream(Global.GraphicsDevice, imageStream);
            imageStream.Dispose();

            var pictureFont = Global.LoadFont("Cheepicus12.font").GetFont(Font.FontSizes.Quarter);
            //var pictureFont = Global.FontDefault;

            // Configure the logo
            _consoleImage = image.ToSurface(pictureFont, true);
            _consoleImage.ViewPort = new Rectangle(0, 5, _consoleImage.Width, 116);
            _consoleImage.Position = new Point(80, 5);
            //Children.Add(_consoleImage);
            //Children.Insert(2, _consoleImage);
            _base.Children.Add(_consoleImage);
        }

        public void Update()
        {
            _statsConsole.Clear();
            _rewardConsole.Clear();

            switch (_mogwai.Adventure.AdventureState)
            {
                case AdventureState.Extended:
                    ChangeImage("travel.png");
                    _statsConsole.Print(1, 1, $"Next Block get's your mogwai deeper into the dungeon.");
                    _statsConsole.Print(1, 2, $"Be prepared {Coloring.Name(_mogwai.Name)} for the stage,");
                    _statsConsole.Print(1, 3, $"you killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters so far,");
                    _statsConsole.Print(1, 4, $"but {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsAlive))} monsters still waiting for you.");
                    _statsConsole.Print(1, 5, $"Stay save {Coloring.Name(_mogwai.Name)}. May your soul survive!");
                    break;

                case AdventureState.Completed:
                    ChangeImage("victory.png");
                    _statsConsole.Print(1, 1, $"Gratulations {Coloring.Name(_mogwai.Name)} you're a true hero!");
                    _statsConsole.Print(1, 2, $"You killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters and explored this dungeon, ");
                    _statsConsole.Print(1, 3, $"no monster was able to take it up with your power!");
                    _statsConsole.Print(1, 4, $"Your name is history, HURRAY HURRAY {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(1, 5, $"   {Coloring.Green("HURRAY HURRAY")}!");
                    AddStatistic(true);
                    break;

                case AdventureState.Failed:
                    ChangeImage("fail.png");
                    _statsConsole.Print(1, 1, $"You where not able to finish this dungeon, {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(1, 2, $"You killed {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsDead))} monsters,");
                    _statsConsole.Print(1, 3, $"but {Coloring.DoDmg(_mogwai.Adventure.MonstersList.Count(p => p.IsAlive))} monsters where to much for you.");
                    _statsConsole.Print(1, 4, $"May your soul be free now, {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(1, 5, $"   {Coloring.Red("GAME OVER")}!");
                    AddStatistic(false);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void AddStatistic(bool winner)
        {
            _rewardConsole.Fill(DefaultForeground, new Color(0,0,0,0), null);
            _rewardConsole.Print(1, 0, "Statistics",Color.Gold);
            _rewardConsole.Print(1, 1, $"Monsters");
            
            _rewardConsole.Print(12, 1, $"{_mogwai.Adventure.MonstersList.Count(p => p.IsDead)}/{_mogwai.Adventure.MonstersList.Count()}".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 1, $"owned", Color.Gainsboro);
            _rewardConsole.Print(1, 2, $"Exploration");
            _rewardConsole.Print(12, 2, $"{_mogwai.Adventure.Map.GetExplorationState():0}".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 2, $"%", Color.Gainsboro);
            _rewardConsole.Print(1, 3, $"Treasures");
            _rewardConsole.Print(12, 3, $"0/0".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 3, $"found", Color.Gainsboro);
            _rewardConsole.Print(1, 4, $"Overall");
            _rewardConsole.Print(12, 4, $"1".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 4, $"x", Color.Gainsboro);
            _rewardConsole.Print(1, 5, "".PadRight(25,'-'), Color.Gainsboro);
            if (winner)
            {
                _rewardConsole.Print(1, 6, $"REWARD", Color.DarkGray);
                _rewardConsole.Print(12, 6, $"?".PadLeft(6), Color.Gold);
                _rewardConsole.Print(19, 6, $"Gold", Color.Gold);
                _rewardConsole.Print(1, 7, $"REWARD", Color.DarkGray);
                _rewardConsole.Print(12, 7, $"?".PadLeft(6), Color.Gold);
                _rewardConsole.Print(19, 7, $"XP", Color.Gold);
            }
            else
            {
                _rewardConsole.Print(1, 6, $"FAILED", Color.Red);
            }
        }
    }
}