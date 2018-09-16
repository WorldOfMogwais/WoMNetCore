using Microsoft.Xna.Framework;
using SadConsole.Input;
using System;
using System.Linq;
using System.Security;
using log4net.Repository.Hierarchy;
using SadConsole;
using SadConsole.Controls;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMWallet.Node;
using Console = SadConsole.Console;
using WoMFramework.Game.Model;
using WoMWallet.Tool;

namespace WoMSadGui.Consoles
{
    public class PlayScreen : Console
    {
        private MogwaiController _controller;

        private int _glyphX = 0;
        private int _glyphY = 0;
        private int _oldglyphIndex = 185;
        private int _glyphIndex = 185;

        public SadGuiState State { get; set; }

        private PlayStatsConsole _playStatsConsole;
        private PlayInfoConsole _playInfoConsole;
        private ScrollingConsole _log;

        private readonly Mogwai _mogwai;

        private ControlsConsole _command1;
        private ControlsConsole _command2;

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;
            _mogwai = _controller.CurrentMogwai ?? _controller.TestMogwai();

            _playStatsConsole = new PlayStatsConsole(_mogwai, 44, 22);
            _playStatsConsole.Position = new Point(0, 0);
            Children.Add(_playStatsConsole);

            var _custom = new MogwaiConsole("Custom", "", 91, 22);
            _custom.Position = new Point(46, 0);
            Children.Add(_custom);

            _command1 = new ControlsConsole(86, 1);
            _command1.Position = new Point(0, 23);
            _command1.Fill(Color.Transparent, Color.DarkGray, null);
            Children.Add(_command1);

            _log = new ScrollingConsole(85, 13, 100);
            _log.Position = new Point(0, 25);
            Children.Add(_log);

            _playInfoConsole = new PlayInfoConsole(mogwaiController, _mogwai, 49, 14);
            _playInfoConsole.Position = new Point(88, 24);
            Children.Add(_playInfoConsole);

            _command2 = new ControlsConsole(8, 2);
            _command2.Position = new Point(40, 2);
            _command2.Fill(Color.Transparent, Color.DarkGray, null);
            _playInfoConsole.Children.Add(_command2);

            State = SadGuiState.Play;

            Init();
        }

        public void Init()
        {
            IsVisible = true;
            if (_controller.CurrentMogwai != null)
                _controller.RefreshCurrent(1);

            MenuButton(0, "level", DoAction);
            MenuButton(1, "inven", DoAction);
            MenuButton(2, "adven", DoAction);
            MenuButton(3, "modif", DoAction);
            MenuButton(4, "breed", DoAction);
            MenuButton(5, "shop", DoAction);

            var btnNext = new MogwaiButton(8, 1);
            btnNext.Position = new Point(0, 0);
            btnNext.Text = "evo";
            btnNext.Click += (btn, args) => { DoAction(((Button)btn).Text); };
            _command2.Add(btnNext);

            var btnFast = new MogwaiButton(8, 1);
            btnFast.Position = new Point(0, 1);
            btnFast.Text = "evo++";
            btnFast.Click += (btn, args) => { DoAction(((Button)btn).Text); };
            _command2.Add(btnFast);

        }

        private void MenuButton(int buttonPosition, string buttonText, Action<string> buttonClicked)
        {
            var xBtn = 0;
            var xSpBtn = 1;
            var mBtnSize = 7;

            var button = new MogwaiButton(mBtnSize, 1);
            button.Position = new Point(xBtn + buttonPosition * (mBtnSize + xSpBtn), 0);
            button.Text = buttonText;
            button.Click += (btn, args) => { buttonClicked(((Button)btn).Text); };
            _command1.Add(button);
        }

        private void DoAction(string actionStr)
        {
            switch (actionStr)
            {
                case "evo":
                    Evolve();
                    break;
                case "evo++":
                    Evolve(true);
                    break;
            }
            
        }

        public void Evolve(bool fast = false)
        {
            if (!fast)
            {
                _mogwai.Evolve(out var history);
                UpdateLog();
            }
            else
            {
                for (var i = 0; i < 50; i++)
                {
                    if (_mogwai.PeekNextShift != null && _mogwai.PeekNextShift.InteractionType == InteractionType.None)
                    {
                        _mogwai.Evolve(out var history);
                        UpdateLog();
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void UpdateLog()
        {
            foreach (var entry in _mogwai.CurrentShift.History.LogEntries)
            {
                _log.MainCursor.Print(entry.ToString());
                _log.MainCursor.NewLine();
            }
        }

        public void ButtonAdventure()
        {

        }

        internal SadGuiState GetState()
        {
            return State;
        }


        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                State = SadGuiState.Selection;
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Q))
            {
                _glyphIndex++;
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                Print(_glyphX + 2, _glyphY, $"{_glyphIndex}", Color.Yellow);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.A))
            {
                _glyphIndex--;
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                Print(_glyphX + 2, _glyphY, $"{_glyphIndex}", Color.Yellow);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                _glyphX++;
                _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                _glyphX--;
                _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                _glyphY--;
                _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                Print(_glyphX, _glyphY, $"[c:sg {_oldglyphIndex}:1] ", Color.DarkCyan);
                _glyphY++;
                _oldglyphIndex = GetGlyph(_glyphX, _glyphY);
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
                return true;
            }
            Print(0, 0, $"x:{_glyphX} y:{_glyphY} ind:{_glyphIndex}", Color.Cyan);
            return false;
        }

        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
            {

            }

            base.Update(delta);
        }
    }
}
