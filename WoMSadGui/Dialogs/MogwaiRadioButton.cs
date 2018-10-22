using Microsoft.Xna.Framework;
using SadConsole.Controls;

namespace WoMSadGui.Dialogs
{
    public class MogwaiListBox : ListBox
    {
        public MogwaiListBox(int width, int height) : base(width, height)
        {
            Theme.Normal.Background = Color.Black;
            Theme.Normal.Foreground = Color.Cyan;

            Theme.MouseDown.Background = Color.Yellow;
            Theme.MouseDown.Foreground = Color.Black;

            Theme.MouseOver.Background = Color.Cyan;
            Theme.MouseOver.Foreground = Color.Black;

            Theme.Selected.Background = Color.Black;
            Theme.Selected.Foreground = Color.Cyan;

            Theme.Focused.Background = Color.Black;
            Theme.Focused.Foreground = Color.Cyan;
        }
    }

    public class MogwaiRadioButton : RadioButton
    {
        public MogwaiRadioButton(int width, int height) : base(width, height)
        {

            Theme.Normal.Background = Color.Transparent;
            Theme.Normal.Foreground = Color.Cyan;

            Theme.MouseDown.Background = Color.Yellow;
            Theme.MouseDown.Foreground = Color.Black;

            Theme.MouseOver.Background = Color.Cyan;
            Theme.MouseOver.Foreground = Color.Black;

            Theme.Selected.Background = Color.Transparent;
            Theme.Selected.Foreground = Color.Cyan;

            Theme.Focused.Background = Color.Transparent;
            Theme.Focused.Foreground = Color.Cyan;

            Theme.LeftBracket.Normal.Background = Color.Transparent;
            Theme.LeftBracket.Normal.Foreground = Color.Cyan;

            Theme.LeftBracket.MouseDown.Background = Color.Yellow;
            Theme.LeftBracket.MouseDown.Foreground = Color.Black;

            Theme.LeftBracket.MouseOver.Background = Color.Cyan;
            Theme.LeftBracket.MouseOver.Foreground = Color.Black;

            Theme.LeftBracket.Selected.Background = Color.Transparent;
            Theme.LeftBracket.Selected.Foreground = Color.Cyan;

            Theme.LeftBracket.Focused.Background = Color.Transparent;
            Theme.LeftBracket.Focused.Foreground = Color.Cyan;

            Theme.RightBracket.Normal.Background = Color.Transparent;
            Theme.RightBracket.Normal.Foreground = Color.Cyan;

            Theme.RightBracket.MouseDown.Background = Color.Yellow;
            Theme.RightBracket.MouseDown.Foreground = Color.Black;

            Theme.RightBracket.MouseOver.Background = Color.Cyan;
            Theme.RightBracket.MouseOver.Foreground = Color.Black;

            Theme.RightBracket.Selected.Background = Color.Transparent;
            Theme.RightBracket.Selected.Foreground = Color.Cyan;

            Theme.RightBracket.Focused.Background = Color.Transparent;
            Theme.RightBracket.Focused.Foreground = Color.Cyan;

            Theme.CheckedIcon.Normal.Background = Color.Transparent;
            Theme.CheckedIcon.Normal.Foreground = Color.Cyan;

            Theme.CheckedIcon.MouseDown.Background = Color.Yellow;
            Theme.CheckedIcon.MouseDown.Foreground = Color.Black;

            Theme.CheckedIcon.MouseOver.Background = Color.Cyan;
            Theme.CheckedIcon.MouseOver.Foreground = Color.Black;

            Theme.CheckedIcon.Selected.Background = Color.Transparent;
            Theme.CheckedIcon.Selected.Foreground = Color.Cyan;

            Theme.CheckedIcon.Focused.Background = Color.Transparent;
            Theme.CheckedIcon.Focused.Foreground = Color.Cyan;  
            
            Theme.UncheckedIcon.Normal.Background = Color.Transparent;
            Theme.UncheckedIcon.Normal.Foreground = Color.Cyan;

            Theme.UncheckedIcon.MouseDown.Background = Color.Yellow;
            Theme.UncheckedIcon.MouseDown.Foreground = Color.Black;

            Theme.UncheckedIcon.MouseOver.Background = Color.Cyan;
            Theme.UncheckedIcon.MouseOver.Foreground = Color.Black;

            Theme.UncheckedIcon.Selected.Background = Color.Transparent;
            Theme.UncheckedIcon.Selected.Foreground = Color.Cyan;

            Theme.UncheckedIcon.Focused.Background = Color.Transparent;
            Theme.UncheckedIcon.Focused.Foreground = Color.Cyan;  
        }
    }
}