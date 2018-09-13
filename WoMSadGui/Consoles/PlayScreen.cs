using Microsoft.Xna.Framework;
using SadConsole.Input;
using System;
using System.Linq;
using System.Security;
using log4net.Repository.Hierarchy;
using SadConsole;
using SadConsole.Controls;
using WoMFramework.Game.Interaction;
using WoMWallet.Node;
using Console = SadConsole.Console;
using WoMFramework.Game.Model;

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

        private Console _playStatsConsole;
        private Console _info;
        private ScrollingConsole _log;

        private readonly Mogwai _mogwai;

        private ControlsConsole _command2;

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;
            _mogwai = _controller.CurrentMogwai ?? _controller.TestMogwai();

            _playStatsConsole = new PlayStatsConsole(mogwaiController, 44, 22);
            _playStatsConsole.Position = new Point(0, 0);
            Children.Add(_playStatsConsole);

            var _custom = new MogwaiConsole("Custom", "", 91, 22);
            _custom.Position = new Point(46, 0);
            Children.Add(_custom);

            var _command1 = new ControlsConsole(86, 1);
            _command1.Position = new Point(0, 23);
            _command1.Fill(Color.Transparent, Color.DarkGray, null);
            Children.Add(_command1);

            _log = new ScrollingConsole(85, 13, 100);
            _log.Position = new Point(0, 25);
            Children.Add(_log);

            _info = new MogwaiConsole("Info", "", 49, 14);
            _info.Position = new Point(88, 24);
            Children.Add(_info);

            _command2 = new ControlsConsole(8, 1);
            _command2.Position = new Point(40, 0);
            _command2.Fill(Color.Transparent, Color.DarkGray, null);
            _info.Children.Add(_command2);

            State = SadGuiState.Play;

            Init();
            UpdateShift();
        }

        public void Init()
        {
            var btnNext = new Button(8, 1)
            {
                Position = new Point(0,0),
                Text = "next"
            };
            btnNext.Click += (btn, args) =>
            {
                Evolve();
            };
            _command2.Add(btnNext);

            
        }

        public void UpdateShift()
        {
            _info.Print(40, 1, "max.", Color.Cyan);

            _info.Print( 1, 0, "Shift: ", Color.Gainsboro);
            _info.Print( 8, 0, (_mogwai.CurrentShift.IsSmallShift ? "smallShift" : _mogwai.CurrentShift.Interaction.InteractionType.ToString()).PadRight(20), Color.Orange);
            _info.Print( 1, 1, "Next: ", Color.Gainsboro);
            if (_mogwai.Shifts.TryGetValue(_mogwai.Pointer + 1, out var shift))
            {
                _info.Print(8, 1, (shift.IsSmallShift ? "smallShift" : shift.Interaction.InteractionType.ToString()).PadRight(20), Color.LimeGreen);
            }
            else
            {
                _info.Print(8, 1, "...".PadRight(20), Color.Red);
            }
            _info.Print(31, 0, _mogwai.Pointer.ToString().PadLeft(8,'.'));
            _info.Print(31, 1, _mogwai.Shifts.Keys.Max().ToString().PadLeft(8, '.'));

            foreach (var entry in _mogwai.CurrentShift.History.LogEntries)
            {
                _log.MainCursor.Print(entry.ToString());
                _log.MainCursor.NewLine();
            }
            
        }

        public void Evolve()
        {
            _mogwai.Evolve(out var history);

            UpdateShift();
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
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.A))
            {
                _glyphIndex--;
                Print(_glyphX, _glyphY, $"[c:sg {_glyphIndex}:1] ", Color.DarkCyan);
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
            base.Update(delta);
        }
    }
}
