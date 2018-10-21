using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;
using WoMSadGui.Consoles;
using WoMSadGui.Dialogs;
using WoMWallet.Node;
using Game = SadConsole.Game;

namespace WoMSadGui
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    internal class Program
    {
        public const int Width = 141;
        public const int Height = 40;

        private static SplashScreen _splashScreen;
        private static SelectionScreen _selectionScreen;
        private static PlayScreen _playScreen;

        private static LogoConsole _logoConsole;

        private static SadGuiState _state;

        private static MogwaiController _controller;

        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Setup the engine and creat the main window.
            Game.Create("IBM.font", Width, Height);

            // Hook the start event so we can add consoles to the system.
            Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            Game.OnUpdate = Update;

            // Start the game.
            Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            Game.Instance.Dispose();
        }

        private static void Update(GameTime time)
        {
            // As an example, we'll use the F5 key to make the game full screen
            if (Global.KeyboardState.IsKeyReleased(Keys.F5))
            {
                Settings.ToggleFullScreen();
            }
            else if (Global.KeyboardState.IsKeyReleased(Keys.Escape))
            {
                Game.Instance.Exit();
            }

            //return;

            // Called each logic update.
            switch (_state)
            {
                case SadGuiState.Start:
                    _state = LoadBlocksAsync();
                    return;
                case SadGuiState.Action:
                    return;
                case SadGuiState.Login:
                    _state = !_controller.IsWalletCreated ? CreateWallet() : Unlock();
                    return;
                case SadGuiState.Mnemoic:
                    _state = ShowMnemonic();
                    break;
                case SadGuiState.Selection:
                    if (_selectionScreen == null)
                    {
                        if (_playScreen != null)
                        {
                            _playScreen.IsVisible = false;
                        }
                        _playScreen = null;
                        SelectionScreen();
                        break;
                    }
                    _selectionScreen.ProcessKeyboard(Global.KeyboardState);
                    _state = _selectionScreen.GetState();
                    break;
                case SadGuiState.Play:
                    if (_playScreen == null)
                    {
                        if (_selectionScreen != null)
                        {
                            _selectionScreen.IsVisible = false;
                        }
                        _selectionScreen = null;
                        PlayScreen();
                        break;
                    }
                    _playScreen.ProcessKeyboard(Global.KeyboardState);
                    _state = _playScreen.GetState();
                    break;
                case SadGuiState.Fatalerror:
                    _state = Warning("A fatal error happend!", true);
                    break;
                case SadGuiState.Quit:
                    Game.Instance.Exit();
                    break;
            }

        }

        private static SadGuiState LoadBlocksAsync()
        {
            var dialog = new MogwaiProgressDialog("Loading", "caching all mogwai blocks.", 40, 8);
            dialog.AddButton("ok");
            dialog.StartAsync();
            dialog.Button.Click += (btn, args) =>
            {
                if (dialog.IsComplete)
                {
                    dialog.Hide();
                    _state = SadGuiState.Login;
                }
            };
            dialog.Show(true);

            return SadGuiState.Action;
        }

        private static SadGuiState Warning(string warning, bool terminate)
        {
            var dialog = new MogwaiDialog("Warning", warning, 40, 8);
            dialog.AddButton("ok");
            dialog.Button.Click += (btn, args) =>
            {
                _state = terminate ? SadGuiState.Quit : SadGuiState.Selection;
                dialog.Hide();
            };
            dialog.Show(true);

            return SadGuiState.Action;
        }

        private static SadGuiState ShowMnemonic()
        {
            var mnemonicWords = _controller.WalletMnemonicWords;
            var size = mnemonicWords.Length.ToString();
            var dialog = new MogwaiDialog("Show Mnemonic", "[c:g f:LimeGreen:Orange:" + size + "]" + mnemonicWords.ToUpper(), 40, 8);
            dialog.AddButton("memorized");
            dialog.Button.Click += (btn, args) =>
            {
                _state = SadGuiState.Selection;
                dialog.Hide();
            };
            dialog.Show(true);

            return SadGuiState.Action;
        }

        private static SadGuiState CreateWallet()
        {
            var inputDialog = new MogwaiInputDialog("WalletCreation", "new wallet password?", 40, 8);
            inputDialog.AddButton("ok");
            inputDialog.Button.Click += (btn, args) =>
            {
                var password = inputDialog.Input?.Text;
                _controller.CreateWallet(password);
                _state = _controller.IsWalletCreated ? SadGuiState.Mnemoic : SadGuiState.Fatalerror;
                inputDialog.Hide();
            };
            inputDialog.Show(true);

            return SadGuiState.Action;
        }

        private static SadGuiState Unlock()
        {
            var dialog = new MogwaiInputDialog("UnlockWallet", "wallet password?", 40, 8);
            dialog.AddButton("ok");
            dialog.Button.Click += (btn, args) =>
            {
                var password = dialog.Input.Text;
                _controller.UnlockWallet(password);
                _state = _controller.IsWalletUnlocked ? SadGuiState.Selection : SadGuiState.Fatalerror;
                dialog.Hide();
            };
            dialog.Show(true);

            return SadGuiState.Action;
        }

        private static void PlayScreen()
        {
            // clear current childrens
            Global.CurrentScreen.Children.Clear();

            _playScreen = new PlayScreen(_controller, 110, 25) { Position = new Point(2, 1) };

            // Set our new console as the thing to render and process
            Global.CurrentScreen.Children.Add(_playScreen);
        }

        private static void SelectionScreen()
        {
            // clear current childrens
            Global.CurrentScreen.Children.Clear();

            _logoConsole = new LogoConsole(110, 6) { Position = new Point(2, 1) };

            _selectionScreen = new SelectionScreen(_controller, 110, 25) { Position = new Point(2, 9) };

            // Set our new console as the thing to render and process
            Global.CurrentScreen.Children.Add(_logoConsole);
            Global.CurrentScreen.Children.Add(_selectionScreen);
        }

        private static void SplashScreen()
        {
            // Any custom loading and prep. We will use a sample console for now

            //var audioFile = new AudioFileReader("mogwaimusic.mp3");
            //_outputDevice = new WaveOutEvent();
            //_outputDevice.Init(audioFile);
            //_outputDevice.Play();

            _splashScreen = new SplashScreen(140, 30) { IsVisible = true };
            //_splashScreen.SplashCompleted += SplashScreenCompleted;
            Global.CurrentScreen.Children.Add(_splashScreen);
        }


        private static void Init()
        {
            _controller = new MogwaiController();
            SplashScreen();
            _state = SadGuiState.Start;

            //_state = SadGuiState.Play;
            //PlayScreen();
        }

        private static void SplashScreenCompleted()
        {
            Global.CurrentScreen.Children.Clear();
            Global.CurrentScreen.Children.Add(_logoConsole);
            Global.CurrentScreen.Children.Add(_selectionScreen);
            _state = SadGuiState.Start;
        }
    }

    public enum SadGuiState
    {
        Start,
        Login,
        Mnemoic,
        Action,
        Selection,
        Play,
        Fatalerror,
        Quit,
    }
}
