using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

namespace WoMSadGui.Dialogs
{
    public class MogwaiInputDialog : Window
    {
        public Button button;

        public TextBox input;

        public MogwaiInputDialog(string title, string text, int width, int height) : base(width, height)
        {
            Title = "[" + title + "]";
            base.Fill(Color.DarkCyan, Color.Black, null);

            var label1 = new DrawingSurface(Width - 4, height - 6)
            {
                Position = new Point(2, 2)
            };
            label1.Surface.Fill(Color.Cyan, Color.Black, null);
            label1.Surface.Print(0, 0, text);
            Add(label1);

            input = new TextBox(Width - 4);
            input.Position = new Point(2, 4);

            Add(input);

            Center();
        }

        public void AddButon(string text)
        {
            button = new Button(text.Length+2, 1);
            button.Position = new Point(2, 6);
            button.Text = text;
            Add(button);
        }
    }
}
