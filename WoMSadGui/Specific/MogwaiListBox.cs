namespace WoMSadGui.Specific
{
    using Microsoft.Xna.Framework;
    using SadConsole.Controls;

    public class MogwaiListBox : ListBox
    {
        public MogwaiListBox(int width, int height) : base(width, height)
        {
            Theme.BorderTheme.Normal.Background = Color.DarkSlateGray;
            Theme.BorderTheme.Normal.Foreground = Color.Cyan;

            Theme.BorderTheme.MouseDown.Background = Color.DarkSlateGray;
            Theme.BorderTheme.MouseDown.Foreground = Color.Cyan;

            Theme.BorderTheme.MouseOver.Background = Color.DarkSlateGray;
            Theme.BorderTheme.MouseOver.Foreground = Color.Cyan;

            Theme.BorderTheme.Selected.Background = Color.DarkSlateGray;
            Theme.BorderTheme.Selected.Foreground = Color.Cyan;

            Theme.BorderTheme.Focused.Background = Color.DarkSlateGray;
            Theme.BorderTheme.Focused.Foreground = Color.Cyan;

            Theme.ItemTheme.Normal.Background = Color.DarkSlateGray;
            Theme.ItemTheme.Normal.Foreground = Color.Cyan;

            Theme.ItemTheme.MouseDown.Background = Color.DarkSlateGray;
            Theme.ItemTheme.MouseDown.Foreground = Color.Cyan;

            Theme.ItemTheme.MouseOver.Background = Color.Cyan;
            Theme.ItemTheme.MouseOver.Foreground = Color.Black;

            Theme.ItemTheme.Selected.Background = Color.DarkSlateGray;
            Theme.ItemTheme.Selected.Foreground =  Color.Yellow;

            Theme.ItemTheme.Focused.Background = Color.DarkSlateGray;
            Theme.ItemTheme.Focused.Foreground = Color.Cyan;

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


            Theme.ScrollBarTheme.Normal.Background = Color.DarkSlateGray;
            Theme.ScrollBarTheme.Normal.Foreground = Color.Cyan;

            Theme.ScrollBarTheme.MouseDown.Background = Color.DarkSlateGray;
            Theme.ScrollBarTheme.MouseDown.Foreground = Color.Cyan;

            Theme.ScrollBarTheme.MouseOver.Background = Color.DarkSlateGray;
            Theme.ScrollBarTheme.MouseOver.Foreground = Color.Cyan;

            Theme.ScrollBarTheme.Selected.Background = Color.DarkSlateGray;
            Theme.ScrollBarTheme.Selected.Foreground =  Color.Cyan;

            Theme.ScrollBarTheme.Focused.Background = Color.DarkSlateGray;
            Theme.ScrollBarTheme.Focused.Foreground = Color.Cyan;
        }
    }
}
