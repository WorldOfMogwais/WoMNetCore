using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SadConsole;
using SadConsole.DrawCalls;
using SadConsole.Effects;
using SadConsole.Instructions;
using SadConsole.Surfaces;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    internal class WelcomeConsole : MogwaiConsole
    {
        private readonly Basic _consoleImage;
        private readonly Point _consoleImagePosition;

        public WelcomeConsole(int width, int height) : base("Welcome", "to the World of Mogwais", width, height)
        {

            Init();
        }

        public void Init()
        {

        }

    }
}