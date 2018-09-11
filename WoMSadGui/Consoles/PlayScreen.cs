using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using SadConsole.Surfaces;
using System;
using log4net.Repository.Hierarchy;
using WoMWallet.Node;

namespace WoMSadGui.Consoles
{
    public class PlayScreen : SadConsole.Console
    {
        private MogwaiController _controller;

        private readonly Basic _borderSurface;

        private int _glyphIndex = 185;

        public SadGuiState State { get; set; }

        public PlayScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _borderSurface = new Basic(width + 2, height + 2, Font);
            _borderSurface.DrawBox(new Rectangle(0, 0, _borderSurface.Width, _borderSurface.Height),
                                  new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            _borderSurface.Position = new Point(-1, -1);
            Children.Add(_borderSurface);

            State = SadGuiState.Play;

            Init();
        }

        private void Init()
        {

            Print(1, 0, "Hanajaku", Color.Gold);
            Print(10, 0, "Lv.", Color.Gainsboro);
            Print(14, 0, "5", Color.Gold);
            Print(1, 1, "Lawful Good Female", Color.Gainsboro);

            Print(0, 2, "[c:sg 205:50]" + "".PadRight(44), Color.DarkCyan);

            Print(31, 3, "Barbarian", Color.OrangeRed);
            Print(43, 3, "3", Color.Gold);
            Print(31, 4, "Cleric", Color.OrangeRed);
            Print(43, 4, "1", Color.Gold);
            Print(31, 5, "Unspent", Color.Gainsboro);
            Print(43, 5, "1", Color.Aqua);
           
            CreateHealthBar(16, 2, Color.DarkGreen, Color.LimeGreen, 0.60f);
            Print(16, 5, "[c:sg 205:13]" + "".PadRight(13), Color.DarkCyan);
            CreateExperienceBar(31, 6, Color.DarkOrange, Color.Gold, 0.70f);
            Print(31, 6, "[c:sg 205:13]" + "".PadRight(13), Color.DarkCyan);

            PrintStat(1, 3, "STR", 12, 1, 0);
            PrintStat(1, 4, "DEX", 12, 1, 0);
            PrintStat(1, 5, "CON", 12, 1, 0);
            PrintStat(1, 6, "INT", 12, 1, 0);
            PrintStat(1, 7, "WIS", 12, 1, 0);
            PrintStat(1, 8, "CHA", 12, 1, 0);

            PrintStat(16, 6, "AC", 20, 0, 0);
            PrintStat(16, 7, "INI", 3, 0, 0);
            PrintStat(16, 8, "AB", 1, 0, 0);

            Print(0, 9, "[c:sg 205:50]" + "".PadRight(44), Color.DarkCyan);

            PrintStat(1, 10, "FOR", 5, 0, 0);
            PrintStat(1, 11, "REF", 4, 0, 0);
            PrintStat(1, 12, "WIL", 5, 0, 0);

            //StatsWindow(0, 1);
        }

        public void CreateExperienceBar(int x, int y, Color low, Color full, float perc)
        {
            Print(x, y + 1, "823864".PadLeft(9), Color.Orange);
            Print(x + 10, y + 1, "XP", Color.Gainsboro);
            CreateBar(x, y + 2, low, full, perc);
            SetGlyph(x + 13, y + 1, 186, Color.DarkCyan);
            SetGlyph(x + 13, y + 2, 186, Color.DarkCyan);
        }

        public void CreateHealthBar(int x, int y, Color low, Color full, float perc)
        {
            Print(x, y+1, "115/200".PadLeft(9), Color.Orange);
            Print(x + 10, y+1, "HP", Color.Gainsboro);
            CreateBar(x, y + 2, low, full, perc);
            SetGlyph(x + 13, y + 1, 186, Color.DarkCyan);
            SetGlyph(x + 13, y + 2, 186, Color.DarkCyan);
        }

        public void CreateBar(int x, int y, Color low, Color full, float perc)
        {
            Print(x, y, "[c:sg 176:12]" + "".PadRight(13), low);
            SetGlyph(x + 13, y, 186, Color.DarkCyan);
            var p = perc * 36;
            var z = (int)(p / 3);
            var t = (int)(p % 3);
            var g = 176 + t;
            Print(x, y, $"[c:sg 219:{z}]" + "".PadRight(z), full);
            if (t != 0)
            {
                SetGlyph(x + z, y, g, full);
            }
        }

        public void StatsWindow(int x, int y)
        {

            Print(0, 1, "[c:sg 205:12]".PadRight(25), Color.DarkCyan);

            

            SetGlyph(x + 11, y + 1, 186, Color.DarkCyan);
            SetGlyph(x + 11, y + 2, 186, Color.DarkCyan);
            SetGlyph(x + 11, y + 3, 186, Color.DarkCyan);
            SetGlyph(x + 11, y + 4, 186, Color.DarkCyan);
            SetGlyph(x + 11, y + 5, 186, Color.DarkCyan);
            SetGlyph(x + 11, y + 6, 186, Color.DarkCyan);

            Print(0, y + 7, "[c:sg 205:12]".PadRight(25), Color.DarkCyan);

            Print(x + 1, y + 8, " AC", Color.Gainsboro);
            Print(x + 1, y + 9, " AB", Color.Gainsboro);

            Print(5, y + 8, "20", Color.Orange);
            Print(5, y + 9, " 5", Color.Orange);

            Print(0, y + 10, "[c:sg 205:12]".PadRight(25), Color.DarkCyan);
        }

        public void PrintStat(int x, int y, string name, int value, int? mod, int temp)
        {
            int xO = x;
            Color modCol = mod > 0 ? Color.LimeGreen : mod < 0 ? Color.Red : Color.Gray;
            string modSig = mod > 0 ? "+" : mod < 0 ? "-" : "";
            Print(x, y, name.PadLeft(3), Color.Gainsboro);
            x += 4;
            Print(x, y, value.ToString("#0").PadLeft(2), Color.Orange);
            if (mod != null)
            {
                x += 3;
                Print(x, y, (modSig + mod).PadLeft(2), modCol);
            }

            x += 3;
            Print(x, y, temp.ToString().PadLeft(2), Color.Aqua);
            SetGlyph(xO + 13, y, 186, Color.DarkCyan);
        }

        internal SadGuiState GetState()
        {
            return State;
        }


        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                State = SadGuiState.Selection;
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                _borderSurface.SetGlyph(0, 0, ++_glyphIndex, Color.DarkCyan);
                _borderSurface.Print(10, 0, _glyphIndex.ToString(), Color.Yellow);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                _borderSurface.SetGlyph(0, 0, --_glyphIndex, Color.DarkCyan);
                _borderSurface.Print(10, 0, _glyphIndex.ToString(), Color.Yellow);
                return true;
            }
            return false;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
        }
    }
}
