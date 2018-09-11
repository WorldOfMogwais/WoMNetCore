using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;

namespace WoMSadGui.Dialogs
{
    public class MogwaiDialog : Window
    {
        public Button Button;

        public MogwaiDialog(string title, string text, int width, int height) : base(width, height)
        {
            Title = "[" + title + "]";
            Fill(Color.DarkCyan, Color.Black, null);

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
            Button = new Button(text.Length + 2, 1);
            Button.Position = new Point(2, 6);
            Button.Text = text;
            Add(Button);
        }
    }
}
