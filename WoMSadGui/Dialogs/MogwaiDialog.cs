using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMSadGui.Dialogs
{
    public class MogwaiDialog : Window
    {
        public Button button;

        public MogwaiDialog(string title, string text, int width, int height) : base(width, height)
        {
            Title = "[" + title + "]";
            base.Fill(Color.DarkCyan, Color.Black, null);

            var label1 = new DrawingSurface(Width - 4, height - 5)
            {
                Position = new Point(2, 2)
            };
            label1.Surface.Fill(Color.Cyan, Color.Black, null);
            label1.Surface.Print(0, 0, new ColoredString(text));
            Add(label1);

            Center();
        }

        public void AddButon(string text)
        {
            button = new Button(text.Length + 2, 1);
            button.Position = new Point(2, 6);
            button.Text = text;
            Add(button);
        }
    }
}
