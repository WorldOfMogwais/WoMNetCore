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
    internal class CustomAdventureChoose : MogwaiConsole
    {
        private readonly Basic _consoleImage;
        private readonly Point _consoleImagePosition;

        public CustomAdventureChoose(int width, int height) : base("Adventure", " select your adventure ", width, height)
        {

            Init();
        }

        public void Init()
        {

        }

    }
}