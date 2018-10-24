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
            Theme.MouseDown.Background = Color.Yellow;
            Theme.MouseDown.Foreground = Color.Black;
            Theme.MouseOver.Foreground = Color.Black;
            Theme.Selected.Background = Color.DarkSlateGray;
            Theme.Focused.Background = Color.DarkSlateGray;

            SetColor(Color.Cyan);
        }

        public void ResetColor()
        {
            SetColor(Color.Cyan);
        }

        public void SetColor(Color mainColor)
        {
            Theme.Normal.Foreground = mainColor;
            Theme.MouseOver.Background = mainColor;
            Theme.Selected.Foreground = mainColor;
            Theme.Focused.Foreground = mainColor;
        }
    }
}
