using SadConsole;
using Microsoft.Xna.Framework;
using WoMSadGui.Consoles;
using WoMSadGui.Dialogs;
using WoMWallet.Node;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;

namespace WoMSadGui
{

    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public const int Width = 141;
        public const int Height = 40;

        private static SplashScreen _splashScreen;
        private static SelectionScreen _selectionScreen;
        private static PlayScreen _playScreen;

        private static LogoConsole _logoConsole;

        private static SadGuiState _state;

        private static MogwaiController _controller;

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Setup the engine and creat the main window.
            SadConsole.Game.Create("IBM.font", Width, Height);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Hook the update event that happens each frame so we can trap keys and respond.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();

            //
            // Code here will not run until the game window closes.
            //

            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime time)
        {
            // As an example, we'll use the F5 key to make the game full screen
            if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.F5))
            {
                Settings.ToggleFullScreen();
            }
            else if (Global.KeyboardState.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                SadConsole.Game.Instance.Exit();
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
                    if (!_controller.IsWalletCreated)
                    {
                        _state = CreateWallet();
                    }
                    else
                    {
                        _state = Unlock();
                    }
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
                    SadConsole.Game.Instance.Exit();
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
                if (terminate)
                {
                    _state = SadGuiState.Quit;
                }
                else
                {
                    _state = SadGuiState.Selection;
                }
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
                if (_controller.IsWalletCreated)
                {
                    _state = SadGuiState.Mnemoic;
                }
                else
                {
                    _state = SadGuiState.Fatalerror;
                }
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
                if (_controller.IsWalletUnlocked)
                {
                    _state = SadGuiState.Selection;
                }
                else
                {
                    _state = SadGuiState.Fatalerror;
                }
                dialog.Hide();
            };
            dialog.Show(true);

            return SadGuiState.Action;
        }

        private static void PlayScreen()
        {
            // clear current childrens
            Global.CurrentScreen.Children.Clear();

            _playScreen = new PlayScreen(_controller, 110, 25);
            _playScreen.Position = new Point(2, 1);

            // Set our new console as the thing to render and process
            Global.CurrentScreen.Children.Add(_playScreen);
        }

        private static void SelectionScreen()
        {
            // clear current childrens
            Global.CurrentScreen.Children.Clear();

            _logoConsole = new LogoConsole(110, 6);
            _logoConsole.Position =  new Point(2, 1);

            _selectionScreen = new SelectionScreen(_controller, 110, 25);
            _selectionScreen.Position = new Point(2, 9);

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

            _splashScreen = new SplashScreen(140, 30);
            _splashScreen.IsVisible = true;
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
        Quit
    }
}
