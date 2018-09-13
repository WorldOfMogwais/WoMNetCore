using Microsoft.Xna.Framework;
using SadConsole.Input;
using System;
using System.Security;
using log4net.Repository.Hierarchy;
using WoMWallet.Node;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    public class PlayScreen : SadConsole.Console
    {
        private MogwaiController _controller;

        private int _glyphX = 0;
        private int _glyphY = 0;
        private int _oldglyphIndex = 185;
        private int _glyphIndex = 185;

        public SadGuiState State { get; set; }

        public Console _playStatsConsole;

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;

            _playStatsConsole = new PlayStatsConsole(mogwaiController, 44, 22);
            _playStatsConsole.Position = new Point(0, 0);
            Children.Add(_playStatsConsole);

            var _custom = new MogwaiConsole("Custom", "", 91, 22);
            _custom.Position = new Point(46, 0);
            Children.Add(_custom);

            var _log = new MogwaiConsole("Log", "", 86, 14);
            _log.Position = new Point(0, 24);
            Children.Add(_log);

            var _info = new MogwaiConsole("Info", "", 49, 14);
            _info.Position = new Point(88, 24);
            Children.Add(_info);

            State = SadGuiState.Play;

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
