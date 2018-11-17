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

        private Font _pictureFont;

        private Font _statsFont;

        public CustomAdventureStats(Mogwai mogwai, int width, int height) : base("Adventure Extension", "This is a temp. summary expect more here", width, height)
        {

            _pictureFont = Global.LoadFont("AutoReiv.font").GetFont(Font.FontSizes.Quarter);
            _statsFont = Global.LoadFont("AutoReiv.font").GetFont(Font.FontSizes.One);
            //var pictureFont = Global.FontDefault;


            _base = new Console(width, height)  {Position = new Point(0, 0)};
            Children.Add(_base);

            _statsConsole = new Console(width, height)  {Position = new Point(2, 2), Font = _statsFont};
            Children.Add(_statsConsole);

            _rewardConsole = new Console(27, 9)  {Position = new Point(5, 26), Font = _statsFont };
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

            // Configure the logo
            _consoleImage = image.ToSurface(_pictureFont, true);
            _consoleImage.ViewPort = new Rectangle(0, 100, _consoleImage.Width, 176);
            _consoleImage.Position = new Point(200, 7);
            //Children.Add(_consoleImage);
            //Children.Insert(2, _consoleImage);
            _base.Children.Add(_consoleImage);
        }

        public void Update()
        {
            _statsConsole.Clear();
            _rewardConsole.Clear();
            //_statsConsole.Fill(DefaultForeground, new Color(20, 20, 20, 50), null);
            switch (_mogwai.Adventure.AdventureState)
            {
                case AdventureState.Extended:
                    ChangeImage("travel.png");
                    break;

                case AdventureState.Completed:
                    ChangeImage("victory.png");
                    _statsConsole.Print(1, 1, $"Gratulations {Coloring.Name(_mogwai.Name)}, you're a true hero!", Color.Brown);
                    _statsConsole.Print(1, 2, $" Promise me you'll always remember that you're", Color.Brown);
                    _statsConsole.Print(1, 3, $"braver than you believe, stronger than you seem,", Color.Brown);
                    _statsConsole.Print(1, 4, $"and smarter than you think.", Color.Brown);
                    _statsConsole.Print(1, 6, $" You're history, {Coloring.Name(_mogwai.Name)}, HURRAY!", Color.Gold);
                    _statsConsole.Print(1, 8, $"                         Book of Mogwai, Ch.1337", Color.DarkGray);
                    _statsConsole.Print(1, 9, $"'they will stand up as one and fight as legions!'", Color.OrangeRed);

                    AddStatistic(true);
                    break;

                case AdventureState.Failed:
                    ChangeImage("fail.png");
                    //_statsConsole.Print(1, 1, $"         1         2         3         4         5         6");
                    //_statsConsole.Print(1, 2, $"123456789012345678901234567890123456789012345678901234567890");
                    _statsConsole.Print(1, 1, $"You died trying to free the world of this");
                    _statsConsole.Print(1, 2, $"terrible injustice, {Coloring.Name(_mogwai.Name)}!");
                    _statsConsole.Print(1, 3, $" The brave do not live forever but the cautious");
                    _statsConsole.Print(1, 4, $"do not live at all.");
                    _statsConsole.Print(1, 6, $" May your soul be free now, {Coloring.Name(_mogwai.Name)}!", Color.Gold);
                    _statsConsole.Print(11, 8, $"             Book of Mogwai, Ch.7487", Color.DarkGray);
                    _statsConsole.Print(11, 9, $"'A hero died and the world lost hope.'", Color.OrangeRed);
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
            _rewardConsole.Print(1, 1, $"Monsters", Color.DarkGray);

            _rewardConsole.Print(12, 1, $"{_mogwai.Adventure.MonstersList.Count(p => p.IsDead)}/{_mogwai.Adventure.MonstersList.Count()}".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 1, $"owned", Color.Gainsboro);
            _rewardConsole.Print(1, 2, $"Exploration", Color.DarkGray);
            _rewardConsole.Print(12, 2, $"{_mogwai.Adventure.Map.GetExplorationState():0}".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 2, $"%", Color.Gainsboro);
            _rewardConsole.Print(1, 3, $"Treasures", Color.DarkGray);
            _rewardConsole.Print(12, 3, $"0/0".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 3, $"found", Color.Gainsboro);
            _rewardConsole.Print(1, 4, $"Overall", Color.DarkGray);
            _rewardConsole.Print(12, 4, $"1".PadLeft(6), Color.LimeGreen);
            _rewardConsole.Print(19, 4, $"x", Color.Gainsboro);
            _rewardConsole.Print(1, 5, $"Dungeon", Color.DarkGray);
            _rewardConsole.Print(12, 5, winner ? "CLEARED" : "FAILED".PadLeft(8), winner ? Color.LimeGreen : Color.Red);
            //_rewardConsole.Print(19, 5, $"", Color.Gainsboro);
            
            _rewardConsole.Print(1, 7, $"REWARD", Color.Gainsboro);
            _rewardConsole.Print(12, 7, $"?".PadLeft(6), Color.Gold);
            _rewardConsole.Print(19, 7, $"Gold", Color.Gold);
            _rewardConsole.Print(1, 8, $"REWARD", Color.Gainsboro);
            _rewardConsole.Print(12, 8, $"?".PadLeft(6), Color.Gold);
            _rewardConsole.Print(19, 8, $"XP", Color.Gold);

        }
    }
}