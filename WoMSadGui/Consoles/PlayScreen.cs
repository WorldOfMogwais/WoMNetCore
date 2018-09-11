using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using SadConsole.Surfaces;
using System;
using WoMWallet.Node;

namespace WoMSadGui.Consoles
{
    public class PlayScreen : SadConsole.Console
    {
        private MogwaiController _controller;

        private readonly Basic _borderSurface;

        public SadGuiState State { get; set; }

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _borderSurface = new Basic(width + 2, height + 2, Font);
            _borderSurface.DrawBox(new Rectangle(0, 0, _borderSurface.Width, _borderSurface.Height),
                                  new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            _borderSurface.Position = new Point(-1, -1);
            Children.Add(_borderSurface);

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
            return false;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
        }
    }
}
