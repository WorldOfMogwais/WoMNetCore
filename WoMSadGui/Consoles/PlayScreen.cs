namespace WoMSadGui.Consoles
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using SadConsole;
    using SadConsole.Controls;
    using SadConsole.Surfaces;
    using Specific;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WoMFramework.Game;
    using WoMFramework.Game.Enums;
    using WoMFramework.Game.Interaction;
    using WoMFramework.Game.Model.Mogwai;
    using WoMWallet.Node;
    using Console = SadConsole.Console;
    using Keyboard = SadConsole.Input.Keyboard;

    public class TestControls : ControlsConsole
    {
        public Basic BorderSurface;

        public TestControls(int width, int height) : base(width, height)
        {
            BorderSurface = new Basic(width + 2, height + 2, Font);
            BorderSurface.DrawBox(new Rectangle(0, 0, BorderSurface.Width, BorderSurface.Height),
                new Cell(Color.DarkCyan, Color.TransparentBlack));
            BorderSurface.Position = new Point(-1, -1);
            Children.Add(BorderSurface);
        }
    }

    public class PlayScreen : Console
    {
        private enum PlayScreenState
        {
            Welcome, Shop, Adventure, AdventureStats,
            AdventureChoose
        }

        private readonly MogwaiController _controller;

        private int _glyphX;
        private int _glyphY;
        private int _oldglyphIndex = 185;
        private int _glyphIndex = 185;

        public SadGuiState State { get; set; }

        private PlayScreenState _playScreenState;

        private Console _custom;

        private readonly MogwaiConsole _welcome;
        private readonly CustomAdventureStats _adventureStats;
        private readonly MogwaiConsole _shop;
        private readonly CustomAdventure _adventure;
        private readonly CustomAdventureChoose _adventureChoose;

        private readonly ScrollingConsole _log;

        private readonly Mogwai _mogwai;

        private readonly TestControls _command1;
        private readonly ControlsConsole _command2;

        private MogwaiButton _btnEvolve;
        private MogwaiButton _btnFast;

        private readonly Dictionary<string, MogwaiButton> _playScreenButtons;

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;
            MogwaiKeys mogwaiKeys = _controller.CurrentMogwaiKeys ?? _controller.TestMogwaiKeys();
            _mogwai = mogwaiKeys.Mogwai;

            _playScreenButtons = new Dictionary<string, MogwaiButton>();

            var playStatsConsole = new PlayStatsConsole(_mogwai, 44, 22) { Position = new Point(0, 0) };
            Children.Add(playStatsConsole);

            _welcome = new CustomWelcome(91, 22) { Position = new Point(46, 0) };

            _adventureChoose = new CustomAdventureChoose(mogwaiController, 91, 22) { Position = new Point(46, 0) };


            _shop = new CustomShop(_mogwai, 91, 22) { Position = new Point(46, 0) };
            _adventure = new CustomAdventure(mogwaiController, mogwaiKeys, 91, 22) { Position = new Point(46, 0) };
            _adventureStats = new CustomAdventureStats(_mogwai, 91, 22) { Position = new Point(46, 0) };

            //var logFont = Global.LoadFont("Bakus8.font").GetFont(Font.FontSizes.One);
            _log = new ScrollingConsole(85, 13, 100, null) { Position = new Point(0, 25) };
            Children.Add(_log);

            var playInfoConsole = new PlayInfoConsole(mogwaiController, mogwaiKeys, 49, 14) { Position = new Point(88, 24) };
            Children.Add(playInfoConsole);

            _command1 = new TestControls(86, 1) { Position = new Point(0, 23) };
            _command1.Fill(Color.Transparent, Color.Black, null);
            Children.Add(_command1);

            _command2 = new ControlsConsole(8, 2) { Position = new Point(40, 2) };
            _command2.Fill(Color.Transparent, Color.DarkGray, null);
            playInfoConsole.Children.Add(_command2);

            State = SadGuiState.Play;

            SetCustomWindowState(PlayScreenState.Welcome);

            Init();
        }

        public void Init()
        {
            IsVisible = true;
            if (_controller.CurrentMogwai != null)
                _controller.RefreshCurrent(1);

            _command1.BorderSurface.SetGlyph(0, 0, 204, Color.DarkCyan);
            _command1.BorderSurface.SetGlyph(0, 1, 186, Color.DarkCyan);
            _command1.BorderSurface.SetGlyph(0, 2, 200, Color.DarkCyan);

            _playScreenButtons.Add("level", MenuButton(0, "level", DoAction));
            _playScreenButtons.Add("inven", MenuButton(1, "inven", DoAction));
            _playScreenButtons.Add("adven", MenuButton(2, "adven", DoAction));
            _playScreenButtons.Add("heal", MenuButton(3, "heal", DoAction));
            _playScreenButtons.Add("modif", MenuButton(4, "modif", DoAction));
            _playScreenButtons.Add("breed", MenuButton(5, "breed", DoAction));
            _playScreenButtons.Add("shop", MenuButton(6, "shop", DoAction));

            _btnEvolve = new MogwaiButton(8, 1)
            {
                Position = new Point(0, 0),
                Text = "evolve"
            };
            _btnEvolve.Click += (btn, args) => { DoAction(((Button)btn).Text); };
            _command2.Add(_btnEvolve);

            _btnFast = new MogwaiButton(8, 1)
            {
                Position = new Point(0, 1),
                Text = "evol++"
            };
            _btnFast.Click += (btn, args) => { DoAction(((Button)btn).Text); };
            _command2.Add(_btnFast);
        }

        private void SetCustomWindowState(PlayScreenState newPlayScreenState)
        {
            Children.Remove(_custom);
            switch (newPlayScreenState)
            {
                case PlayScreenState.Welcome:
                    _custom = _welcome;
                    break;
                case PlayScreenState.Shop:
                    _custom = _shop;
                    break;
                case PlayScreenState.AdventureChoose:
                    _custom = _adventureChoose;
                    break;
                case PlayScreenState.Adventure:
                    // disable buttons
                    _playScreenButtons.Values.ToList().ForEach(p => p.IsEnabled = false);
                    _custom = _adventure;
                    _btnEvolve.Text = "next";
                    _btnEvolve.SetColor(Color.DarkOrange);
                    _btnFast.Text = "fini";
                    _btnFast.SetColor(Color.Black);
                    break;
                case PlayScreenState.AdventureStats:
                    _adventureStats.Update();
                    _custom = _adventureStats;
                    _btnEvolve.Text = "evolve";
                    _btnEvolve.ResetColor();
                    if (_mogwai.Adventure == null || !_mogwai.Adventure.IsActive)
                    {
                        _btnFast.Text = "evol++";
                        _btnFast.ResetColor();
                        // enable buttons
                        _playScreenButtons.Values.ToList().ForEach(p => p.IsEnabled = true);
                    }

                    break;
            }

            Children.Add(_custom);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buttonPosition"></param>
        /// <param name="buttonText"></param>
        /// <param name="buttonClicked"></param>
        /// <returns></returns>
        private MogwaiButton MenuButton(int buttonPosition, string buttonText, Action<string> buttonClicked)
        {
            var xBtn = 0;
            var xSpBtn = 1;
            var mBtnSize = 7;

            var button = new MogwaiButton(mBtnSize, 1)
            {
                Position = new Point(xBtn + buttonPosition * (mBtnSize + xSpBtn), 0),
                Text = buttonText
            };
            button.Click += (btn, args) => { buttonClicked(((Button)btn).Text); };
            _command1.Add(button);
            _command1.SetGlyph(xBtn + mBtnSize + buttonPosition * (mBtnSize + xSpBtn), 0, 186, Color.DarkCyan);
            _command1.BorderSurface.SetGlyph(xBtn + mBtnSize + 1 + buttonPosition * (mBtnSize + xSpBtn), 0, 203, Color.DarkCyan);
            for (var i = 0; i < mBtnSize; i++)
            {
                _command1.BorderSurface.SetGlyph(xBtn + i + 1 + buttonPosition * (mBtnSize + xSpBtn), 2, 205, Color.DarkCyan);
            }

            _command1.BorderSurface.SetGlyph(xBtn + mBtnSize + 1 + buttonPosition * (mBtnSize + xSpBtn), 2, 202, Color.DarkCyan);

            return button;
        }

        private void DoAction(string actionStr)
        {
            MogwaiOptionDialog dialog;
            switch (actionStr)
            {
                case "evolve":
                    Evolve();
                    break;
                case "next":
                    EvolveAdventure();
                    break;
                case "evol++":
                    Evolve(true);
                    break;
                case "shop":
                    SetCustomWindowState(PlayScreenState.Shop);
                    break;
                case "adven":
                    SetCustomWindowState(PlayScreenState.AdventureChoose);
                    break;
                case "level":
                    dialog = new MogwaiOptionDialog("Leveling", "Currently up for leveling?", DoInteraction, 40, 12);
                    dialog.AddRadioButtons("levelingAction", new List<string[]> {
                        new[] {"levelclass", "Class levels."},
                        new[] {"levelability", "Ability levels."}
                    });
                    dialog.Show(true);
                    break;
                case "heal":
                    dialog = new MogwaiOptionDialog("Healing", "Choose Divine prayer?", DoInteraction, 40, 12);
                    dialog.AddRadioButtons("levelingAction", new List<string[]> {
                        new[] {"heal", "Heal Injuries"},
                        new[] {"reviving", "Reviving"}
                    });
                    dialog.Show(true);
                    break;
            }
        }

        private void DoInteraction(string actionStr)
        {
            switch (actionStr)
            {
                case "testroom":
                    LogInConsole(
                        _controller.Interaction(new AdventureAction(AdventureType.TestRoom, DifficultyType.Average,
                            _mogwai.CurrentLevel))
                            ? "Successful sent mogwai to test room! Wait for interaction locks."
                            : "Failed to send mogwai to test room!");
                    break;
                case "dungeon":
                    LogInConsole(
                        _controller.Interaction(new AdventureAction(AdventureType.Dungeon, DifficultyType.Average,
                            _mogwai.CurrentLevel))
                            ? "Successful sent mogwai to dungeon! Wait for interaction locks."
                            : "Failed to send mogwai to test room!");
                    break;
                case "levelclass":
                    var dialog = new MogwaiOptionDialog("Leveling", "Currently up for leveling?", DoClassLevel, 40, 17);
                    dialog.AddRadioButtons("levelingAction", new List<string[]> {
                        new[] { "Barbarian", "Barbarian"},
                        new[] { "Bard", "Bard"},
                        new[] { "Cleric", "Cleric"},
                        new[] { "Druid", "Druid"},
                        new[] { "Fighter", "Fighter"},
                        new[] { "Monk", "Monk"},
                        new[] { "Paladin", "Paladin"},
                        new[] { "Ranger", "Ranger"},
                        new[] { "Rogue", "Rogue"},
                        new[] { "Sorcerer", "Sorcerer"},
                        new[] { "Wizard", "Wizard"}
                    });
                    dialog.Show(true);
                    break;
                case "heal":
                    LogInConsole(
                        _controller.Interaction(new SpecialAction(SpecialType.Heal, SpecialSubType.None, CostType.Medium))
                            ? "Successful made prayer for divine healing! Wait for interaction locks."
                            : "Failed to pray for mogwai divine heal!");
                    break;
                case "reviving":
                    LogInConsole(
                        _controller.Interaction(new SpecialAction(SpecialType.Reviving, SpecialSubType.None, CostType.High))
                            ? "Successful made prayer for divine reviving! Wait for interaction locks."
                            : "Failed to pray for mogwai divine reviving!");
                    break;
                default:
                    var warning = new MogwaiDialog("NotImplemented", $"DoInteraction {actionStr}!", 40, 6);
                    warning.AddButton("ok");
                    warning.Button.Click += (btn, args) =>
                    {
                        warning.Hide();
                    };
                    warning.Show(true);
                    break;
            }
        }

        private void DoClassLevel(string classTypeStr)
        {
            if (!_mogwai.CanLevelClass(out _))
            {
                LogInConsole("Mogwai can\'t class level, no level ups!");
                return;
            }

            if (!Enum.TryParse<ClassType>(classTypeStr, true, out ClassType classType))
            {
                LogInConsole($"Invalid class, please check {classTypeStr}!");
                return;
            }

            if (_mogwai.Classes.Count >= 2 && _mogwai.GetClassLevel(classType) == 0)
            {
                LogInConsole("Mogwais of this generation can\'t level more then two classes!");
                return;
            }

            if (!_controller.Interaction(new LevelingAction(LevelingType.Class, classType, _mogwai.CurrentLevel,
                _mogwai.GetClassLevel(classType))))
            {
                LogInConsole("Failed to class leveling!");
                return;
            }

            LogInConsole("Successful sent mogwai out for class leveling.");
        }

        private void EvolveAdventure()
        {
            _adventure.Stop();

            if (_mogwai.CanEvolveAdventure)
            {
                _mogwai.EvolveAdventure();
            }

            if (_adventure != null)
            {
                SetCustomWindowState(PlayScreenState.AdventureStats);
                return;
            }
        }

        public void Evolve(bool fast = false)
        {
            if (!fast)
            {
                if (_mogwai.Evolve())
                {
                    if (_mogwai.Adventure != null)
                    {
                        if (!(_custom is CustomAdventure))
                        {
                            SetCustomWindowState(PlayScreenState.Adventure);
                        }

                        _adventure.Start(_mogwai.Adventure);
                    }
                    else
                    {
                        SetCustomWindowState(PlayScreenState.Welcome);
                    }

                    UpdateLog();
                }
            }
            else if (_mogwai.PeekNextShift != null)
            {
                SetCustomWindowState(PlayScreenState.Welcome);

                _log.Reset();
                while (_mogwai.PeekNextShift != null && _mogwai.PeekNextShift.IsSmallShift && _mogwai.Evolve())
                {
                    UpdateLog();
                }
            }
        }

        private void UpdateLog()
        {
            foreach (LogEntry entry in _mogwai.CurrentShift.History.LogEntries)
            {
                _log.MainCursor.Print(entry.ToString());
                _log.MainCursor.NewLine();
            }
        }

        public void PushLog(LogEntry logEntry)
        {
            _log.MainCursor.Print(logEntry.ToString());
            _log.MainCursor.NewLine();
        }

        internal SadGuiState GetState()
        {
            return State;
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Keys.Enter))
            {
                State = SadGuiState.Selection;
                return true;
            }

            if (Program.DEBUG)
            {
                if (state.IsKeyReleased(Keys.Q))
                {
                    _glyphIndex++;
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    Print(_glyphX + 2, _glyphY, $"{_glyphIndex}", Color.Yellow);
                    return true;
                }

                if (state.IsKeyReleased(Keys.A))
                {
                    _glyphIndex--;
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    Print(_glyphX + 2, _glyphY, $"{_glyphIndex}", Color.Yellow);
                    return true;
                }

                if (state.IsKeyReleased(Keys.Right))
                {
                    Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                    _glyphX++;
                    _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    return true;
                }

                if (state.IsKeyReleased(Keys.Left))
                {
                    Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                    _glyphX--;
                    _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    return true;
                }

                if (state.IsKeyReleased(Keys.Up))
                {
                    Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                    _glyphY--;
                    _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    return true;
                }

                if (state.IsKeyReleased(Keys.Down))
                {
                    Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                    _glyphY++;
                    _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                    Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                    return true;
                }

                Print(0, 0, $"x:{_glyphX} y:{_glyphY} ind:{_glyphIndex}", Color.Cyan);
            }

            return false;
        }

        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
            {
                switch (_custom)
                {
                    case CustomAdventure _:
                        if (_adventure.Adventure != null)
                        {
                            _adventure.UpdateGame();
                        }

                        break;
                }
            }

            base.Update(delta);
        }

        public void LogInConsole(string msg)
        {
            _log.MainCursor.Print(msg);
            _log.MainCursor.NewLine();
        }
    }
}
