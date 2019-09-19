namespace WoMSadGui.Specific
{
    using Microsoft.Xna.Framework;
    using SadConsole.Controls;

    public class MogwaiInputDialog : MogwaiDialog
    {
        public TextBox Input;

        public MogwaiInputDialog(string title, string text, int width, int height) : base(title, text, width, height)
        {
            Input = new TextBox(Width - 4) { Position = new Point(2, 4) };

            Add(Input);

            Center();
        }
    }
}
