using System;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Model.Mogwai;
using WoMWallet.Node;
using WoMWallet.Tool;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    public class PlayInfoConsole : Console
    {
        private readonly Basic _borderSurface;
        private readonly MogwaiController _controller;
        private readonly MogwaiKeys _mogwaiKeys;
        private readonly Mogwai _mogwai;

        public PlayInfoConsole(MogwaiController mogwaiController, MogwaiKeys mogwaiKeys, int width, int height) : base(width, height)
        {
            _controller = mogwaiController;
            _mogwaiKeys = mogwaiKeys;
            _mogwai = mogwaiKeys.Mogwai;

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

            Print(1, 2, $"[{_mogwai.Pointer.ToString().PadLeft(7, '.')}]", Color.Gainsboro);
            Print(11, 2, $"{_mogwai.CurrentShift.InteractionType}".PadRight(20), Color.Lime);

            for (int i = 1; i < 3; i++)
            {
                if (_mogwai.Shifts.TryGetValue(_mogwai.Pointer + i, out var shift))
                {
                    Print(1, 2 + i, $"[{(_mogwai.Pointer + i).ToString().PadLeft(7, '.')}]", Color.Gainsboro);
                    Print(11, 2 + i, $"{shift.InteractionType}".PadRight(20), Color.LimeGreen);
                }
                else
                {
                    Print(1, 2 + i, $"[{"".ToString().PadLeft(7, '.')}]", Color.Red);
                    Print(11, 2 + i, $"None".PadRight(20), Color.Red);
                }
            }
 
            Print(31, 2, _mogwai.Pointer.ToString().PadLeft(8, '.'));
            Print(31, 3, _mogwai.Shifts.Keys.Max().ToString(CultureInfo.InvariantCulture).PadLeft(8, '.'));

            var lastBlock = _controller.WalletLastBlock;
            if (lastBlock != null)
            {
                Print(1, 0, _controller.WalletLastBlock.Height.ToString("#######0").PadLeft(8), Color.DeepSkyBlue);
                Print(10, 0, "Block", Color.White);
                var localTime = DateUtil.GetBlockLocalDateTime(_controller.WalletLastBlock.Time);
                var localtimeStr = localTime.ToString(CultureInfo.InvariantCulture);
                var t = DateTime.Now.Subtract(localTime);
                var timeStr = $"[c:r f:springgreen]{t:hh\\:mm\\:ss}[c:u]";
                Print(16, 0, localtimeStr + " " + timeStr, Color.Gainsboro);
            }

            var balance = _mogwaiKeys.Balance;
            var balanceStr = balance < 1000 ? balance.ToString("##0.0000").PadLeft(8) : "RICH".PadRight(8);
            Print(10, 13, "MOG", Color.Gainsboro);
            Print(1, 13, balanceStr, Color.Orange);
            var addr = _controller.CurrentMogwai != null ? _mogwaiKeys.Address : "MFHRD3E7m6FdJA5HTEDQTMMzFMg9LXNTwA";
            Print(1, 10, "Interactions: ", Color.Gainsboro);

            if (_mogwaiKeys.InteractionLock.Count == 0)
            {
                Print(15, 10, _mogwaiKeys.InteractionLock.Count.ToString().PadLeft(2), Color.LimeGreen);
                Print(18, 10, "Locked", Color.Gainsboro);
                Print(14, 13, $"[c:g f:limegreen:orange:limegreen:{addr.Length}]" + addr);
            }
            else
            {
                Print(15, 10, _mogwaiKeys.InteractionLock.Count.ToString().PadLeft(2), Color.Red);
                Print(18, 10, "Locked", Color.Gainsboro);
                Print(1, 11, _mogwaiKeys.InteractionLock.Values.First().GetInfo().PadRight(48).Substring(0, 48), Color.Red);
                Print(14, 13, $"[c:g f:red:orange:red:{addr.Length}]" + addr);
            }


        }

        public void Init()
        {
            _borderSurface.Print(0, 2, "[c:sg 204:1] ", Color.DarkCyan);
            _borderSurface.Print(50, 2, "[c:sg 185:1] ", Color.DarkCyan);
            Print(0, 1, "[c:sg 205:49]" + "".PadRight(49), Color.DarkCyan);

            _borderSurface.Print(0, 13, "[c:sg 204:1] ", Color.DarkCyan);
            _borderSurface.Print(50, 13, "[c:sg 185:1] ", Color.DarkCyan);
            Print(0, 12, "[c:sg 205:49]" + "".PadRight(49), Color.DarkCyan);

        }
    }
}