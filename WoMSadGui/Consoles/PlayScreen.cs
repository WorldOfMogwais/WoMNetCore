using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using WoMWallet.Node;
using WoMWallet.Tool;

namespace WoMSadGui.Consoles
{
    public class PlayScreen : SadConsole.Console
    {
        private MogwaiController controller;

        private Basic borderSurface;

        public SadGuiState State { get; set; }

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            borderSurface = new Basic(width + 2, height + 2, base.Font);
            borderSurface.DrawBox(new Rectangle(0, 0, borderSurface.Width, borderSurface.Height),
                                  new Cell(Color.DarkCyan, Color.Black), null, SurfaceBase.ConnectedLineThick);
            borderSurface.Position = new Point(-1, -1);
            Children.Add(borderSurface);

            State = SadGuiState.PLAY;
        }

        internal SadGuiState GetState()
        {
            return State;
        }


        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                State = SadGuiState.SELECTION;
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
