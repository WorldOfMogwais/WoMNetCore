namespace WoMSadGui.Specific
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Controls;

    public class MogwaiDialog : Window
    {
        public Button Button;

        public Button ButtonCancel;

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

        public void AddButton(string text, bool cancel = false)
        {
            int button = (Width - text.Length) / 2;
            if (cancel)
            {
                var textCancel = "Cancel";
                ButtonCancel = new MogwaiButton(textCancel.Length + 2, 1)
                {
                    Position = new Point(Width - textCancel.Length - 4, Height - 2),
                    Text = textCancel
                };
                ButtonCancel.Click += (btn, args) =>
                {
                    Hide();
                };
                Add(ButtonCancel);
                button = 2;
            }

            Button = new MogwaiButton(text.Length + 2, 1)
            {
                Position = new Point(button, Height - 2),
                Text = text
            };
            Add(Button);
        }
    }
}
