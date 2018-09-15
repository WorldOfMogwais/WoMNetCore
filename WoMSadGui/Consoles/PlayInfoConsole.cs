using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Model;
using WoMWallet.Node;
using WoMWallet.Tool;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    public class PlayInfoConsole : Console
    {
        private readonly Basic _borderSurface;
        private MogwaiController _controller;
        private readonly Mogwai _mogwai;


        public PlayInfoConsole(MogwaiController mogwaiController, Mogwai mogwai, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;
            _mogwai = mogwai;

            _borderSurface = new Basic(width + 2, height + 2, Font);
            _borderSurface.DrawBox(new Rectangle(0, 0, _borderSurface.Width, _borderSurface.Height),
                new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            _borderSurface.Position = new Point(-1, -1);
            Children.Add(_borderSurface);

            Init();
        }

        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
            {
                UpdateScreen();
            }

            base.Update(delta);
        }

        public void UpdateScreen()
        {

            Print(1, 2, "Shift: ", Color.Gainsboro);


            Print(8, 2, _mogwai.CurrentShift.InteractionType.ToString().PadRight(20), Color.Orange);
            Print(1, 3, "Next: ", Color.Gainsboro);
            if (_mogwai.Shifts.TryGetValue(_mogwai.Pointer + 1, out var shift))
            {
                Print(8, 3, shift.InteractionType.ToString().PadRight(20), Color.LimeGreen);
            }
            else
            {
                Print(8, 3, "...".PadRight(20), Color.Red);
            }
            Print(31, 2, _mogwai.Pointer.ToString().PadLeft(8, '.'));
            Print(31, 3, _mogwai.Shifts.Keys.Max().ToString().PadLeft(8, '.'));

            var lastBlock = _controller.WalletLastBlock;
            if (lastBlock != null)
            {
                Print(1, 0, _controller.WalletLastBlock.Height.ToString("#######0").PadLeft(8), Color.DeepSkyBlue);
                Print(10, 0, "Block", Color.White);
                var localTime = DateUtil.GetBlockLocalDateTime(_controller.WalletLastBlock.Time);
                var localtimeStr = localTime.ToString();
                var t = DateTime.Now.Subtract(localTime);
                var timeStr = $"[c:r f:springgreen]{string.Format("{0:hh\\:mm\\:ss}", t)}[c:u]";
                Print(16, 0, localtimeStr + " " + timeStr, Color.Gainsboro);
            }
        }

        public void Init()
        {
            _borderSurface.Print(0, 2, "[c:sg 204:1] ", Color.DarkCyan);
            _borderSurface.Print(50, 2, "[c:sg 185:1] ", Color.DarkCyan);
            Print(0, 1, "[c:sg 205:49]" + "".PadRight(49), Color.DarkCyan);

        }
    }
}