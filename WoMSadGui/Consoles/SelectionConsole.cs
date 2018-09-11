using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using SadConsole.Effects;
using SadConsole.Input;
using SadConsole.Surfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using WoMWallet.Node;
using WoMWallet.Tool;

namespace WoMSadGui.Consoles
{
    public class SelectionScreen : SadConsole.Console
    {
        private Basic borderSurface;

        private int glyphIndex = 185;

        private MogwaiController controller;

        private ControlsConsole controlsConsole;
        private MogwaiConsole infoConsole;
        private MogwaiConsole logConsole;

        public int headerPosition;
        public int trailerPosition;

        public bool IsReady { get; set; } = false;

        public SadGuiState State {get;set;}

        public int windowOffset = 0;
        public int maxRows = 21;

        public SelectionScreen(MogwaiController mogwaiController, int width, int height) : base(width, height)
        {
            borderSurface = new Basic(width + 2, height + 2, base.Font);
            borderSurface.DrawBox(new Rectangle(0, 0, borderSurface.Width, borderSurface.Height),
                                  new Cell(Color.DarkCyan, Color.Black), null, SurfaceBase.ConnectedLineThick);
            borderSurface.Position = new Point(-1, -1);
            Children.Add(borderSurface);

            controlsConsole = new ControlsConsole(110, 1);
            controlsConsole.Position = new Point(0, 24);
            controlsConsole.Fill(Color.DarkCyan, Color.Black, null);
            Children.Add(controlsConsole);

            infoConsole = new MogwaiConsole("Info", "", 24, 38);
            infoConsole.Position = new Point(113, -8);
            Children.Add(infoConsole);

            logConsole = new MogwaiConsole("Log", "", 110, 3);
            logConsole.Position = new Point(0, 27);
            Children.Add(logConsole);

            headerPosition = 1;
            trailerPosition = height - 2;

            CreateHeader();
            CreateTrailer();

            controller = mogwaiController;
        }

        public void Init()
        {
            IsReady = true;
            Print(65, 0, $"Deposit:", Color.DarkCyan);
            Print(74, 0, $"[c:g f:LimeGreen:Orange:34]{controller.DepositAddress}");
            controller.Refresh(1);
            State = SadGuiState.SELECTION;

            infoConsole.Cursor.NewLine();
            infoConsole.Cursor.Print(new ColoredString(".:|Keyboard Commands|:.", Color.White, Color.Black));
            infoConsole.Cursor.NewLine();
            infoConsole.Cursor.NewLine();
            InfoPrint(".C.", "create mogwai key");
            InfoPrint(".S.", "send 5 mog to addr");
            InfoPrint(".B.", "bind mogwai 1 mog");
            InfoPrint(".P.", "play mogwai");
            InfoPrint(".S.", "show mogwai");
            InfoPrint(".L.", "log pub keys file");
            InfoPrint(".T.", "tag for send multi");
        }

        private void InfoPrint(string key, string descritpion)
        {
            infoConsole.Cursor.Print(new ColoredString(key, Color.Orange, Color.Black));
            infoConsole.Cursor.Print(new ColoredString(">>", Color.Lime, Color.Black));
            infoConsole.Cursor.Print(new ColoredString(descritpion, Color.DeepSkyBlue, Color.Black));
            infoConsole.Cursor.NewLine();
        }

        private void CreateHeader()
        {
            Print(0, headerPosition, "[c:sg 205:110]".PadRight(124), Color.DarkCyan);
            borderSurface.SetGlyph(0, headerPosition + 1, 204, Color.DarkCyan);
            borderSurface.SetGlyph(111, headerPosition + 1, 185, Color.DarkCyan);
            //SetGlyph(03, headerPosition, 185, Color.DarkCyan);

            Print(03, headerPosition, " Address ");
            SetGlyph(39, headerPosition, 185, Color.DarkCyan);
            Print(40, headerPosition, " State ");
            SetGlyph(48, headerPosition, 185, Color.DarkCyan);
            Print(49, headerPosition, " Funds ");
            SetGlyph(60, headerPosition, 185, Color.DarkCyan);
            Print(61, headerPosition, " Name ");
            SetGlyph(74, headerPosition, 185, Color.DarkCyan);
            Print(75, headerPosition, " Rating ");
            SetGlyph(84, headerPosition, 185, Color.DarkCyan);
            Print(85, headerPosition, " Level ");
            SetGlyph(93, headerPosition, 185, Color.DarkCyan);
            Print(94, headerPosition, " Gold ");
        }

        private void AddButton(int index, string text, Action<string> buttonClicked)
        {
            SetGlyph(10 + (index * 11), trailerPosition, 203, Color.DarkCyan);
            controlsConsole.SetGlyph(10 + (index * 11), 0, 186, Color.DarkCyan);
            borderSurface.SetGlyph(11 + (index * 11), trailerPosition + 3, 202, Color.DarkCyan);
            var txt = text;
            var button = new Button(8, 1)
            {
                Position = new Point(1 + (index * 11), 0),
                Text = txt
            };
            button.Click += (btn, args) =>
            {
                buttonClicked(((Button)btn).Text);
            };
            controlsConsole.Add(button);
        }

        private void CreateTrailer()
        {
            // 202 203 1086
            Print(0, trailerPosition, "[c:sg 205:110]".PadRight(124), Color.DarkCyan);
            borderSurface.SetGlyph(0, trailerPosition + 1, 204, Color.DarkCyan);
            borderSurface.SetGlyph(111, trailerPosition + 1, 185, Color.DarkCyan);

            AddButton(0, "create", DoAction);
            AddButton(1, "send", DoAction);
            AddButton(2, "bind", DoAction);
            AddButton(3, "show", DoAction);
            AddButton(4, "play", DoAction);
        }

        internal SadGuiState GetState()
        {
            return State;
        }

        private void DoAction(string actionStr)
        {
            switch(actionStr)
            {
                case "create":
                    controller.NewMogwaiKeys();
                    LogInConsole("TASK", "created new mogwaikeys.");
                    break;
                case "send":
                    if (controller.HasMogwayKeys)
                    {
                        if (controller.SendMog())
                        {
                            LogInConsole("DONE", $"sending mogs to address {controller.CurrentMogwayKeys.Address}.");
                        }
                        else
                        {
                             LogInConsole("FAIL", $"couldn't send mogs, see log file for more information.");
                        }                        
                    }
                    break;
                case "bind":
                    if (controller.HasMogwayKeys)
                    {
                        if (controller.BindMogwai())
                        {
                            LogInConsole("DONE", $"binding mogwai on address {controller.CurrentMogwayKeys.Address}.");
                        }
                        else
                        {
                             LogInConsole("FAIL", $"couldn't bind mogwai, see log file for more information.");
                        }  
                      }
                    break;
                case "show":
                    break;
                case "play":
                    if (controller.CurrentMogwayKeys != null && controller.CurrentMogwayKeys.Mogwai != null)
                    {
                        State = SadGuiState.PLAY;
                    } else
                    {
                        LogInConsole("FAIL", $"make sure to choosse a mogwai before trying to play.");
                    }
                    break;             
                default:
                    break;
            }

            // clear all taggs after actions
            controller.ClearTag();
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Enter))
            {
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.C))
            {
                DoAction("create");
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.S))
            {
                DoAction("send");
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.B))
            {
                DoAction("bind");
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.P))
            {
                DoAction("play");
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.L))
            {
                controller.PrintMogwaiKeys();
                LogInConsole("TASK", "loging public keys into a file.");
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.T))
            {
                controller.Tag();
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Down))
            {
                controller.Next();
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Up))
            {
                controller.Previous();
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                borderSurface.SetGlyph(0, 0, ++glyphIndex, Color.DarkCyan);
                borderSurface.Print(10, 0, glyphIndex.ToString(), Color.Yellow);
                return true;
            }
            else if (state.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                borderSurface.SetGlyph(0, 0, --glyphIndex, Color.DarkCyan);
                borderSurface.Print(10, 0, glyphIndex.ToString(), Color.Yellow);
                return true;
            }

            return false;
        }

        public void LogInConsole(string type, string msg)
        {
            string color = "khaki";
            if (type == "DONE")
            {
                color = "limegreen";
            } else if (type == "FAIL")
            {
                color = "red";
            }
            var time = DateTime.Now.ToLocalTime().ToLongTimeString();
            logConsole.Cursor.Print($"[c:r f:{color}]{time}[[c:r f:khaki]{type}[c:r f:{color}]]:[c:r f:gray] {msg}");
            logConsole.Cursor.NewLine();
        }

        public override void Update(TimeSpan delta)
        {
            if (IsReady)
            {

                decimal deposit = controller.GetDepositFunds();
                var depositStr = deposit < 10000 ? deposit.ToString("###0.0000").PadLeft(9) : "TYCOON".PadRight(9);
                var lastBlock = controller.WalletLastBlock;
                if (lastBlock != null)
                {
                    Print(1, 0, controller.WalletLastBlock.Height.ToString("#######0").PadLeft(8), Color.DeepSkyBlue);
                    Print(10, 0, "Block", Color.White);
                    var localTime = DateUtil.GetBlockLocalDateTime(controller.WalletLastBlock.Time);
                    var localtimeStr = localTime.ToString();
                    var t = DateTime.Now.Subtract(localTime);
                    Print(16, 0, localtimeStr + " [   s]", Color.Gainsboro);
                    Print(18 + localtimeStr.Length, 0, t.TotalSeconds.ToString("##0").PadLeft(3), Color.SpringGreen);
                }
                Print(45, 0, "Funds:", Color.DarkCyan);
                Print(52, 0, depositStr, Color.Orange);

                if (windowOffset > controller.CurrentMogwayKeysIndex)
                {
                    windowOffset = controller.CurrentMogwayKeysIndex;
                }
                else if(maxRows < controller.CurrentMogwayKeysIndex + 1)
                {
                    windowOffset = controller.CurrentMogwayKeysIndex + 1 - maxRows;
                }
                
                // only updated if we have keys
                if (controller.HasMogwayKeys)
                {
                    var list = controller.MogwaiKeysList;
                    for (int i = windowOffset; i < list.Count && i - windowOffset < maxRows; i++)
                    {
                        var mogwaiKeys = list[i];
                        var pos = i - windowOffset;
                        PrintRow(pos + headerPosition + 1, mogwaiKeys, mogwaiKeys.Address == controller.CurrentMogwayKeys.Address, controller.TaggedMogwaiKeys.Contains(mogwaiKeys));
                    }
                    //PrintRow(pointer + headerPosition + 1, list[pointer], true);
                }
            }
            base.Update(delta);
        }

        private void PrintRow(int index, MogwaiKeys mogwaiKeys, bool selected = false, bool tagged = false)
        {
            int aPos = 4;
            int sPos = 41;
            int fPos = 50;
            int nPos = 62;
            int rPos = 76;
            int lPos = 86;
            int gPos = 95;
            var balance = mogwaiKeys.Balance;
            var balanceStr = balance < 1000 ? balance.ToString("##0.0000").PadLeft(8) : "RICH".PadRight(8);
            var mogwai = mogwaiKeys.Mogwai;
            var nameStr = mogwai != null ? mogwai.Name.PadRight(11) : "".PadRight(11, '.');
            var rateStr = mogwai != null ? mogwai.Rating.ToString("#0.00").PadRight(7) : "".PadRight(7, '.');
            var levlStr = mogwai != null ? mogwai.CurrentLevel.ToString("##0").PadRight(5) : "".PadRight(5, '.');
            var goldStr = mogwai != null ? mogwai.Wealth.Gold.ToString("#####0.00").PadRight(10) : "".PadRight(10, '.');

            Print(3, index, !tagged ? " " : ">", !tagged ? Color.Black : Color.DeepSkyBlue);
            Print(1, index, !selected ? "  " : "=>", !selected ? Color.Black : Color.SpringGreen);

            Color standard = GetColorStandard(mogwaiKeys.MogwaiKeysState, selected);
            Color extState = GetMogwaiKeysStateColor(mogwaiKeys.MogwaiKeysState, selected);

            Print(aPos, index, mogwaiKeys.Address.PadRight(36), standard);
            Print(sPos, index, mogwaiKeys.MogwaiKeysState.ToString().PadRight(6), extState);
            Print(fPos, index, balanceStr, extState);
            Print(nPos, index, nameStr, standard);
            Print(rPos, index, rateStr, standard);
            Print(lPos, index, levlStr, standard);
            Print(gPos, index, goldStr, standard);
        }

        private Color GetColorStandard(MogwaiKeysState mogwaiKeysState, bool Selected)
        {
            switch (mogwaiKeysState)
            {
                case MogwaiKeysState.NONE:
                    return Selected ? Color.WhiteSmoke : Color.DarkGray;
                case MogwaiKeysState.WAIT:
                    return Selected ? Color.WhiteSmoke : Color.DarkGray;
                case MogwaiKeysState.READY:
                    return Selected ? Color.Sienna : Color.SaddleBrown;
                case MogwaiKeysState.CREATE:
                    return Selected ? Color.Gold : Color.DarkGoldenrod;
                case MogwaiKeysState.BOUND:
                    return Selected ? Color.Gold : Color.DarkGoldenrod;
                default:
                    return Color.MediumSeaGreen;
            }
        }

        private Color GetMogwaiKeysStateColor(MogwaiKeysState mogwaiKeysState, bool Selected)
        {
            switch (mogwaiKeysState)
            {
                case MogwaiKeysState.NONE:
                    return Selected ? Color.Red : Color.DarkRed;
                case MogwaiKeysState.WAIT:
                    return Selected ? Color.RoyalBlue : Color.SteelBlue;
                case MogwaiKeysState.READY:
                    return Selected ? Color.LimeGreen : Color.DarkGreen;
                case MogwaiKeysState.CREATE:
                    return Selected ? Color.RoyalBlue : Color.SteelBlue;
                case MogwaiKeysState.BOUND:
                    return Selected ? Color.Gold : Color.DarkGoldenrod;
                default:
                    return Color.RoyalBlue;
            }
        }
    }
}
