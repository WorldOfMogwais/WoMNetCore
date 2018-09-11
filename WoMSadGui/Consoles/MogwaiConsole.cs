using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;

namespace WoMSadGui.Consoles
{
    public class MogwaiConsole : Console
    {
        public MogwaiConsole(string title, string footer, int width, int height) : base(width, height)
        {
            Cursor.Position = new Point(0, 0);
   
            var borderSurface = new Basic(width + 2, height + 2, Font);
            borderSurface.DrawBox(new Rectangle(0, 0, borderSurface.Width, borderSurface.Height), 
                                  new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            borderSurface.Position = new Point(-1, -1);
            borderSurface.Print(2, 0, title, Color.DarkCyan, Color.Black);
            borderSurface.Print(width - footer.Length - 2, height + 1, footer, Color.DarkCyan, Color.Black);
            Children.Add(borderSurface);

            
        }

    }
}
