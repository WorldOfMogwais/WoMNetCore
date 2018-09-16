using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using WoMSadGui.Consoles;

namespace WoMSadGui.Dialogs
{
    public class MogwaiInputDialog : MogwaiDialog
    {
        public TextBox Input;

        public MogwaiInputDialog(string title, string text, int width, int height) : base(title, text, width, height)
        {
            Input = new TextBox(Width - 4);
            Input.Position = new Point(2, 4);

            Add(Input);

            Center();
        }

    }
}
