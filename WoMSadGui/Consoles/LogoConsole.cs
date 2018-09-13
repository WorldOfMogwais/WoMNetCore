using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMSadGui.Art;

namespace WoMSadGui.Consoles
{
    public class LogoConsole : Console
    {
        public LogoConsole(int width, int height) : base(width, height)
        {
            const string footer = "Mogwaicoin Team 2018";
            Cursor.Position = new Point(0, 0);
   
            var borderSurface = new Basic(width + 2, height + 2, Font);
            borderSurface.DrawBox(new Rectangle(0, 0, borderSurface.Width, borderSurface.Height), 
                                  new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            borderSurface.Position = new Point(-1, -1);
            borderSurface.Print(width - footer.Length - 2, height + 1, footer, Color.DarkCyan, Color.Black);
            Children.Add(borderSurface);

            for (var i = 0; i < Ascii.Logo.Length; i++)
            {
                var str = Ascii.Logo[i];
                Print(4, i, $"[c:g b:0,0,0:Black:DarkCyan:DarkGoldenRod:DarkRed:Black:0,0,0:{str.Length}][c:g f:LimeGreen:Orange:{str.Length}]" + str, Color.Cyan, Color.Black);
            }

        }

    }
}
