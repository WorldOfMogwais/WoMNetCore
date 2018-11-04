using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.Controls;
using SadConsole.DrawCalls;
using SadConsole.Effects;
using SadConsole.Instructions;
using SadConsole.Surfaces;
using SadConsole.Themes;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;
using WoMWallet.Node;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    internal class CustomAdventureChoose : MogwaiConsole
    {
        private ControlsConsole _travelControl;

        private List<MogwaiChooseButton> _consoleList;

        private MogwaiChooseButton _current = null;

        private DifficultyType _currentDifficultyType;

        private readonly MogwaiController _controller;
        
        private readonly Mogwai _mogwai;

        public CustomAdventureChoose(MogwaiController mogwaiController, int width, int height) : base("Adventure", "select your adventure", width, height)
        {
            _controller = mogwaiController;
            var mogwaiKeys = _controller.CurrentMogwaiKeys ?? _controller.TestMogwaiKeys();
            _mogwai = mogwaiKeys.Mogwai;

            _consoleList = new List<MogwaiChooseButton>();
            _currentDifficultyType = DifficultyType.Easy;

            Init();
        }

        public void Init()
        {
            _consoleList.Add(CreateChoice(0, 0, "Hollow Mountain", "Hollow Mountain is the largest peak on Rivenrake Island, which lies at " +
                                                  "the northwestern edge of the Varisian Gulf", "icon2s.png", AdventureType.TestRoom));
            _consoleList.Add(CreateChoice(1, 0, "The Manstone Caverns", "The underground complex, a network of caverns accessible only via " +
                                                       "the Darklands or through well-defended chambers connected to hideouts " +
                                                       "above.", "icon1s.png", AdventureType.Chamber));
            _consoleList.Add(CreateChoice(2, 0, "Gallowspire", "Located in the Hungry Mountains in southwestern Ustalav, the tower of Gallowspire " +
                                              "stands as a crumbling testament to the power of the immortal lich Tar-Baphon.", "icon3s.png", AdventureType.Dungeon));
            _consoleList.Add(CreateChoice(0, 1, "The Pyramid of Kamaria", "Kamaria, which dominates the southern skyline of the Osirian " +
                                                       "city of An. Dedicated to the only ruler of the ancient kingdom.", "icon4s.png", AdventureType.Battle));
            _consoleList.Add(CreateChoice(1, 1, "The King Karamoss", "A thousand years ago, the city of Absalom faced one of its most exotic foes. " +
                                                    "The mysterious wizard Karamoss.", "icon5s.png", AdventureType.Quest));

            _travelControl = new ControlsConsole(43, 7);
            _travelControl.Position = new Point(47, 14);
            _travelControl.Fill(Color.Transparent, new Color(05,05,05,255), null);
            Children.Add(_travelControl);

            _travelControl.Print(1, 1, "Adventure: ", Color.Gainsboro);
            _travelControl.Print(1, 2, "Difficulty:", Color.Gainsboro);

            var button = new MogwaiButton(10, 1)
            {
                Position = new Point(4, 6),
                Text = "TRAVEL"
            };
            button.Click += (btn, args) => { DoAdventure(); };
            _travelControl.Add(button);

            var bDec = new MogwaiButton(3, 1)
            {
                Position = new Point(34, 2),
                Text = "-"
            };
            bDec.Click += (btn, args) => { DoDifficulty(false); };
            _travelControl.Add(bDec);

            var bInc = new MogwaiButton(3, 1)
            {
                Position = new Point(38, 2),
                Text = "+"
            };
            bInc.Click += (btn, args) => { DoDifficulty(true); };
            _travelControl.Add(bInc);

            DoDifficulty(false);
            DoAction(AdventureType.Dungeon);
        }

        private void DoDifficulty(bool b)
        {
            if (b)
            {
                _currentDifficultyType =
                    (int) _currentDifficultyType < Enum.GetValues(typeof(DifficultyType)).Length - 2
                        ? (DifficultyType) _currentDifficultyType + 1
                        : _currentDifficultyType;
            }
            else
            {
                _currentDifficultyType =
                    (int)_currentDifficultyType > -1 ? _currentDifficultyType - 1
                        : _currentDifficultyType;
            }

            _travelControl.Print(13, 2, _currentDifficultyType.ToString().PadRight(20), GetDifficultyColor(_currentDifficultyType));
           
        }

        private Color GetDifficultyColor(DifficultyType currentDifficultyType)
        {
            switch (_currentDifficultyType)
            {
                case DifficultyType.Easy:
                    return Color.LightBlue;
                case DifficultyType.Average:
                    return Color.LimeGreen;
                case DifficultyType.Challenging:
                    return Color.Yellow;
                case DifficultyType.Hard:
                    return Color.Orange;
                case DifficultyType.Epic:
                    return Color.Red;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DoAdventure()
        {
            if (_current == null)
            {
                return;
            }

            (this.Parent as PlayScreen)?.LogInConsole(
                _controller.Interaction(new AdventureAction(AdventureType.Dungeon, _currentDifficultyType,
                    _mogwai.CurrentLevel))
                    ? "Successful sent mogwai to dungeon! Wait for interaction locks."
                    : "Failed to send mogwai to test room!");
        }

        private MogwaiChooseButton CreateChoice(int index, int row, string name, string description, string pathIcon, AdventureType adventureType)
        {
            var choiceConsole = new Console(32, 7);
            choiceConsole.Position = new Point(13 + row * 45, 0 + index * 7);
            choiceConsole.Fill(Color.TransparentBlack, Color.Black, null);
            choiceConsole.Print(0, 0, name, Color.White);
            choiceConsole.Print(0, 1, $"[c:g b:darkred:black:black:{description.Length}]" + description, Color.DarkGray);
            Children.Add(choiceConsole);

            var controls = new ControlsConsole(10, 5);
            controls.Position = new Point(-12, 1);
            controls.Fill(Color.Transparent, Color.DarkGray, null);
            choiceConsole.Children.Add(controls);
            var button = new MogwaiChooseButton(10, 5)
            {
                Position = new Point(0, 0)
            };
            button.Click += (btn, args) => { DoAction(adventureType); };
            controls.Add(button);
            button.Unselect();

            // Load the logo
            var imageStream = TitleContainer.OpenStream(pathIcon);
            var image = Texture2D.FromStream(Global.GraphicsDevice, imageStream);
            imageStream.Dispose();

            var pictureFont = Global.LoadFont("Cheepicus12.font").GetFont(Font.FontSizes.Quarter);
            // Configure the logo
            var consoleImage = image.ToSurface(pictureFont, true);
            consoleImage.Position = new Point(85 + row * 75, 12 + 30 * index);
            //consoleImage.Tint = Color.DarkSlateBlue;
            controls.Children.Add(consoleImage);

            return button;
        }

        private void DoAction(AdventureType adventureType)
        {
            _current?.Unselect();

            _current = _consoleList[(int) adventureType];

            _current?.Select();
            var b = adventureType != AdventureType.Dungeon;
            var str = $"{adventureType} {(b ? $"[LOCKED]" : "")}";
            _travelControl.Print(13, 1, str.PadRight(30), b ? Color.Red : Color.Orange);

            //Children.Add();
        }
    }
}