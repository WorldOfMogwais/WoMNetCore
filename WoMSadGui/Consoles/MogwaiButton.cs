using Microsoft.Xna.Framework;
using SadConsole.Controls;

namespace WoMSadGui.Consoles
{
    public class MogwaiButton : Button
    {
        public MogwaiButton(int width, int height) : base(width, height)
        {
            //Theme.EndCharacterLeft = 186;
            //Theme.EndCharacterRight = 186;
            
            Theme.Normal.Background = Color.DarkSlateGray;
            Theme.Normal.Foreground = Color.Cyan;

            Theme.MouseDown.Background = Color.Yellow;
            Theme.MouseDown.Foreground = Color.Black;

            Theme.MouseOver.Background = Color.Cyan;
            Theme.MouseOver.Foreground = Color.Black;

            Theme.Selected.Background = Color.DarkSlateGray;
            Theme.Selected.Foreground = Color.Cyan;

            Theme.Focused.Background = Color.DarkSlateGray;
            Theme.Focused.Foreground = Color.Cyan;

        }
    }
}
