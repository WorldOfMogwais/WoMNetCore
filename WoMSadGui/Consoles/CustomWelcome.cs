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
    internal class CustomWelcome : MogwaiConsole
    {
        private Basic _consoleImage;
        private Point _consoleImagePosition;

        public CustomWelcome(int width, int height) : base("Welcome", "to the World of Mogwais", width, height)
        {

            Init();
        }

        public void Init()
        {
            // Load the logo
            var imageStream = TitleContainer.OpenStream("womsimple.png");
            var image = Texture2D.FromStream(Global.GraphicsDevice, imageStream);
            imageStream.Dispose();

            var pictureFont = Global.LoadFont("Cheepicus12.font").GetFont(Font.FontSizes.Quarter);
            // Configure the logo
            _consoleImage = image.ToSurface(pictureFont, true);
            _consoleImage.Position = new Point(120, 0);
            //_consoleImage.Tint = Color.Beige;
            Children.Add(_consoleImage);

        }
    }
}