using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NBitcoin.Altcoins;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Monster;
using WoMWallet.Node;
using WoMWallet.Tool;
using Console = SadConsole.Console;
using Keyboard = SadConsole.Input.Keyboard;
using Mogwai = WoMFramework.Game.Model.Mogwai.Mogwai;

namespace WoMSadGui.Consoles
{
    public class SelectionScreen : Console
    {
        private readonly Basic _borderSurface;

        private int _glyphIndex = 185;

        private readonly MogwaiController _controller;

        private readonly ControlsConsole _controlsConsole;
        private readonly MogwaiConsole _infoConsole;
        private readonly MogwaiConsole _logConsole;
        private readonly Console _debugConsole;

        public int HeaderPosition;
        public int TrailerPosition;

        public SadGuiState State { get; set; }

        public int WindowOffset;
        public int MaxRows = 21;

        private int _transferFunds;

        public SelectionScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            _borderSurface = new Basic(width + 2, height + 2, Font);
            _borderSurface.DrawBox(new Rectangle(0, 0, _borderSurface.Width, _borderSurface.Height),
                                  new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            _borderSurface.Position = new Point(-1, -1);
            Children.Add(_borderSurface);

            _controlsConsole = new ControlsConsole(110, 1) { Position = new Point(0, 24) };
            _controlsConsole.Fill(Color.DarkCyan, Color.Black, null);
            Children.Add(_controlsConsole);

            _infoConsole = new MogwaiConsole("Info", "", 24, 38) { Position = new Point(113, -8) };
            Children.Add(_infoConsole);

            _debugConsole = new Console(24, 38) { Position = new Point(113, 22) };
            _debugConsole.Fill(Color.Beige, Color.TransparentBlack, null);
            _debugConsole.Print(1,1, $"Debug Console [{Coloring.Gold("     ")}]:");
            _debugConsole.Print(1,2, $"..armors: {Armors.Instance.AllBuilders().Count}");
            _debugConsole.Print(1,3, $"..weapns: {Weapons.Instance.AllBuilders().Count}");
            _debugConsole.Print(1,4, $"..mnstrs: {Monsters.Instance.AllBuilders().Count}");
            Children.Add(_debugConsole);


            _logConsole = new MogwaiConsole("Log", "", 110, 3) { Position = new Point(0, 27) };
            Children.Add(_logConsole);

            HeaderPosition = 1;
            TrailerPosition = height - 2;

            CreateHeader();
            CreateTrailer();

            _controller = mogwaiController;
            _transferFunds = 2;

            Init();
        }

        public void Init()
        {
            IsVisible = true;
            _controller.RefreshAll(1);

            Print(65, 0, "Deposit:", Color.DarkCyan);
            Print(74, 0, $"[c:g f:LimeGreen:Orange:34]{_controller.DepositAddress}");
            State = SadGuiState.Selection;

            _infoConsole.Cursor.NewLine();
            _infoConsole.Cursor.Print(new ColoredString(".:|Keyboard Commands|:.", Color.White, Color.Black));
            _infoConsole.Cursor.NewLine();
            _infoConsole.Cursor.NewLine();
            InfoPrint(".C.", "create mogwai key");
            InfoPrint(".S.", "send x mog to addr");
            InfoPrint(".B.", "bind mogwai 1 mog");
            InfoPrint(".W.", "(un)watch toggle");
            InfoPrint(".P.", "play mogwai");
            InfoPrint(".L.", "log pub keys file");
            InfoPrint(".T.", "tag for send multi");
            InfoPrint(".I.", "incr funds sent");
        }

        private void InfoPrint(string key, string descritpion)
        {
            _infoConsole.Cursor.Print(new ColoredString(key, Color.Orange, Color.Black));
            _infoConsole.Cursor.Print(new ColoredString(">>", Color.Lime, Color.Black));
            _infoConsole.Cursor.Print(new ColoredString(descritpion, Color.DeepSkyBlue, Color.Black));
            _infoConsole.Cursor.NewLine();
        }

        private void CreateHeader()
        {
            Print(0, HeaderPosition, "[c:sg 205:110]".PadRight(124), Color.DarkCyan);
            _borderSurface.SetGlyph(0, HeaderPosition + 1, 204, Color.DarkCyan);
            _borderSurface.SetGlyph(111, HeaderPosition + 1, 185, Color.DarkCyan);
            //SetGlyph(03, headerPosition, 185, Color.DarkCyan);

            Print(03, HeaderPosition, " Address ");
            SetGlyph(39, HeaderPosition, 185, Color.DarkCyan);
            Print(40, HeaderPosition, " State ");
            SetGlyph(48, HeaderPosition, 185, Color.DarkCyan);
            Print(49, HeaderPosition, " Funds ");
            SetGlyph(60, HeaderPosition, 185, Color.DarkCyan);
            Print(61, HeaderPosition, " Name ");
            SetGlyph(74, HeaderPosition, 185, Color.DarkCyan);
            Print(75, HeaderPosition, " Rating ");
            SetGlyph(84, HeaderPosition, 185, Color.DarkCyan);
            Print(85, HeaderPosition, " Level ");
            SetGlyph(92, HeaderPosition, 185, Color.DarkCyan);
            Print(93, HeaderPosition, " Gold ");
            SetGlyph(104, HeaderPosition, 185, Color.DarkCyan);
            Print(105, HeaderPosition, " % ");
        }

        private void AddButton(int index, string text, Action<string> buttonClicked)
        {
            SetGlyph(10 + index * 11, TrailerPosition, 203, Color.DarkCyan);
            _controlsConsole.SetGlyph(10 + index * 11, 0, 186, Color.DarkCyan);
            _borderSurface.SetGlyph(11 + index * 11, TrailerPosition + 3, 202, Color.DarkCyan);
            var txt = text;
            var button = new MogwaiButton(8, 1)
            {
                Position = new Point(1 + index * 11, 0),
                Text = txt
            };
            button.Click += (btn, args) =>
            {
                buttonClicked(((MogwaiButton)btn).Text);
            };
            _controlsConsole.Add(button);
        }

        private void CreateTrailer()
        {
            // 202 203 1086
            Print(0, TrailerPosition, "[c:sg 205:110]".PadRight(124), Color.DarkCyan);
            _borderSurface.SetGlyph(0, TrailerPosition + 1, 204, Color.DarkCyan);
            _borderSurface.SetGlyph(111, TrailerPosition + 1, 185, Color.DarkCyan);

            AddButton(0, "create", DoAction);
            AddButton(1, "send", DoAction);
            AddButton(2, "bind", DoAction);
            AddButton(3, "watch", DoAction);
            AddButton(4, "play", DoAction);
            AddButton(5, "evolve", DoAction);
        }

        internal SadGuiState GetState()
        {
            return State;
        }

        private void DoAction(string actionStr)
        {
            switch (actionStr)
            {
                case "create":
                    _controller.NewMogwaiKeys();
                    LogInConsole("TASK", "created new mogwaikeys.");
                    break;
                case "send":
                    if (_controller.HasMogwayKeys)
                    {
                        if (_controller.SendMog(_transferFunds))
                        {
                            LogInConsole("DONE", $"sending mogs to address {_controller.CurrentMogwaiKeys.Address}.");
                        }
                        else
                        {
                            LogInConsole("FAIL", "couldn\'t send mogs, see log file for more information.");
                        }
                    }
                    break;
                case "bind":
                    if (_controller.HasMogwayKeys)
                    {
                        if (_controller.BindMogwai())
                        {
                            LogInConsole("DONE", $"binding mogwai on address {_controller.CurrentMogwaiKeys.Address}.");
                        }
                        else
                        {
                            LogInConsole("FAIL", "couldn\'t bind mogwai, see log file for more information.");
                        }
                    }
                    break;
                case "watch":
                    if (_controller.CurrentMogwaiKeys != null)
                    {
                        _controller.WatchToggle();
                    }
                    else
                    {
                        LogInConsole("FAIL", "make sure to choosse a mogwai before trying to play.");
                    }
                    break;
                case "play":
                    if (_controller.CurrentMogwaiKeys?.Mogwai != null && !_controller.CurrentMogwaiKeys.IsLocked)
                    {
                        State = SadGuiState.Play;
                    }
                    else
                    {
                        LogInConsole("FAIL", "make sure to choosse a mogwai before trying to play.");
                    }
                    break;
                case "evolve":
                    if (_controller.CurrentMogwaiKeys?.Mogwai != null)
                    {
                        LogInConsole("DONE", $"auto evolving mogwai {_controller.CurrentMogwaiKeys.Mogwai.Name} now.");
                        _controller.EvolveMogwai();
                    }
                    else
                    {
                        LogInConsole("FAIL", "make sure to choosse a mogwai before trying to play.");
                    }
                    break;
            }

            // clear all taggs after actions
            _controller.ClearTag();
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Keys.Enter))
            {
                DoAction("play");
                return true;
            }

            if (state.IsKeyReleased(Keys.C))
            {
                DoAction("create");
                return true;
            }

            if (state.IsKeyReleased(Keys.S))
            {
                DoAction("send");
                return true;
            }

            if (state.IsKeyReleased(Keys.B))
            {
                DoAction("bind");
                return true;
            }

            if (state.IsKeyReleased(Keys.W))
            {
                DoAction("watch");
                return true;
            }

            if (state.IsKeyReleased(Keys.P))
            {
                DoAction("play");
                return true;
            }

            if (state.IsKeyReleased(Keys.L))
            {
                _controller.PrintMogwaiKeys();
                LogInConsole("TASK", "loging public keys into a file.");
                return true;
            }

            if (state.IsKeyReleased(Keys.T))
            {
                _controller.Tag();
                return true;
            }

            if (state.IsKeyReleased(Keys.I))
            {
                _transferFunds = (_transferFunds - 1) % 7 + 2;
                return true;
            }

            if (state.IsKeyReleased(Keys.Down))
            {
                _controller.Next();
                return true;
            }

            if (state.IsKeyReleased(Keys.Up))
            {
                _controller.Previous();
                return true;
            }

            if (state.IsKeyReleased(Keys.Right))
            {
                _borderSurface.SetGlyph(0, 0, ++_glyphIndex, Color.DarkCyan);
                _borderSurface.Print(10, 0, _glyphIndex.ToString(), Color.Yellow);
                return true;
            }

            if (state.IsKeyReleased(Keys.Left))
            {
                _borderSurface.SetGlyph(0, 0, --_glyphIndex, Color.DarkCyan);
                _borderSurface.Print(10, 0, _glyphIndex.ToString(), Color.Yellow);
                return true;
            }

            return false;
        }

        public void LogInConsole(string type, string msg)
        {
            var color = "khaki";
            if (type == "DONE")
            {
                color = "limegreen";
            }
            else if (type == "FAIL")
            {
                color = "red";
            }
            var time = DateTime.Now.ToLocalTime().ToLongTimeString();
            _logConsole.Cursor.Print($"[c:r f:{color}]{time}[[c:r f:khaki]{type}[c:r f:{color}]]:[c:r f:gray] {msg}");
            _logConsole.Cursor.NewLine();
        }

        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
            {
                var specColor = new Color((40 * _controller.QueueSize) % 256,
                    (255 - (_controller.QueueSize * 10)) % 256, (25 * _controller.QueueSize) % 256);
                _debugConsole.Print(16,1, "ALPHA", specColor);
                var deposit = _controller.GetDepositFunds();
                var depositStr = deposit < 10000 ? deposit.ToString("###0.0000").PadLeft(9) : "TYCOON".PadRight(9);
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
                Print(62, 0, "+" + _transferFunds, Color.LimeGreen);
                Print(45, 0, "Funds:", Color.DarkCyan);
                Print(52, 0, depositStr, Color.Orange);

                if (WindowOffset > _controller.CurrentMogwaiKeysIndex)
                {
                    WindowOffset = _controller.CurrentMogwaiKeysIndex;
                }
                else if (MaxRows < _controller.CurrentMogwaiKeysIndex + 1)
                {
                    WindowOffset = _controller.CurrentMogwaiKeysIndex + 1 - MaxRows;
                }

                // only updated if we have keys
                if (_controller.HasMogwayKeys)
                {
                    var list = _controller.MogwaiKeysList;
                    for (var i = WindowOffset; i < list.Count && i - WindowOffset < MaxRows; i++)
                    {
                        var mogwaiKeys = list[i];
                        var pos = i - WindowOffset;
                        PrintRow(pos + HeaderPosition + 1, mogwaiKeys, mogwaiKeys.Address == _controller.CurrentMogwaiKeys.Address, _controller.TaggedMogwaiKeys.Contains(mogwaiKeys));
                    }
                    //PrintRow(pointer + headerPosition + 1, list[pointer], true);
                }
            }
            base.Update(delta);
        }

        private void PrintRow(int index, MogwaiKeys mogwaiKeys, bool selected = false, bool tagged = false)
        {
            var aPos = 4;
            var sPos = 41;
            var fPos = 50;
            var nPos = 62;
            var rPos = 76;
            var lPos = 86;
            var gPos = 94;
            var pPos = 106;
            var balance = mogwaiKeys.Balance;
            var balanceStr = balance < 1000 ? balance.ToString("##0.0000").PadLeft(8) : "RICH".PadRight(8);
            var mogwai = mogwaiKeys.Mogwai;
            var nameStr = mogwai != null ? mogwai.Name.PadRight(11) : "".PadRight(11, '.');
            var rateStr = mogwai != null ? mogwai.Rating.ToString("#0.00").PadRight(7) : "".PadRight(7, '.');
            var levlStr = mogwai != null ? mogwai.CurrentLevel.ToString("##0").PadRight(4) : "".PadRight(4, '.');
            var goldStr = mogwai != null ? mogwai.Wealth.Gold.ToString("#####0.00").PadRight(9) : "".PadRight(9, '.');
            var progStr = mogwai != null ? ((double) mogwai.CurrentShift.Index /  mogwai.Shifts.Count).ToString("##0%").PadRight(4) : "".PadRight(4, '.');

            Print(3, index, !tagged ? " " : ">", !tagged ? Color.Black : Color.DeepSkyBlue);
            Print(1, index, !selected ? "  " : "=>", !selected ? Color.Black : Color.SpringGreen);

            var standard = GetColorStandard(mogwaiKeys, selected);
            var extState = GetMogwaiKeysStateColor(mogwaiKeys, selected);

            Print(aPos, index, mogwaiKeys.Address.PadRight(36), standard);
            Print(sPos, index, mogwaiKeys.MogwaiKeysState.ToString().PadRight(6), extState);
            Print(fPos, index, balanceStr, extState);
            Print(nPos, index, nameStr, standard);
            Print(rPos, index, rateStr, standard);
            Print(lPos, index, levlStr, standard);
            Print(gPos, index, goldStr, standard);
            Print(pPos, index, progStr, standard);
        }

        private Color GetColorStandard(MogwaiKeys mogwaiKeys, bool selected)
        {
            if (mogwaiKeys.IsUnwatched)
            {
                return selected ? Color.WhiteSmoke : Color.DarkGray;
            }
            switch (mogwaiKeys.MogwaiKeysState)
            {
                case MogwaiKeysState.None:
                    return selected ? Color.WhiteSmoke : Color.DarkGray;
                case MogwaiKeysState.Wait:
                    return selected ? Color.WhiteSmoke : Color.DarkGray;
                case MogwaiKeysState.Ready:
                    return selected ? Color.Sienna : Color.SaddleBrown;
                case MogwaiKeysState.Create:
                    return selected ? Color.Gold : Color.DarkGoldenrod;
                case MogwaiKeysState.Bound:
                    return selected ? Color.Gold : Color.DarkGoldenrod;
                default:
                    return Color.MediumSeaGreen;
            }
        }

        private Color GetMogwaiKeysStateColor(MogwaiKeys mogwaiKeys, bool selected)
        {
            if (mogwaiKeys.IsUnwatched)
            {
                return selected ? Color.WhiteSmoke : Color.DarkGray;
            }
            switch (mogwaiKeys.MogwaiKeysState)
            {
                case MogwaiKeysState.None:
                    return selected ? Color.Red : Color.DarkRed;
                case MogwaiKeysState.Wait:
                    return selected ? Color.RoyalBlue : Color.SteelBlue;
                case MogwaiKeysState.Ready:
                    return selected ? Color.LimeGreen : Color.DarkGreen;
                case MogwaiKeysState.Create:
                    return selected ? Color.RoyalBlue : Color.SteelBlue;
                case MogwaiKeysState.Bound:
                    return selected ? Color.Gold : Color.DarkGoldenrod;
                default:
                    return Color.RoyalBlue;
            }
        }
    }
}
