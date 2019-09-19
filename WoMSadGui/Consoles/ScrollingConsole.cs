namespace WoMSadGui.Consoles
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Controls;
    using SadConsole.Input;
    using SadConsole.Surfaces;
    using System;
    using Console = SadConsole.Console;

    internal class ScrollingConsole : ConsoleContainer
    {
        private readonly Console _mainConsole;                 // The main console that is typed into
        private readonly ControlsConsole _controlsHost;        // The scroll bar host
        private readonly ScrollBar _scrollBar;        // The scroll bar

        private int _scrollingCounter;   // This is a counter to indicate how much buffer is used

        public Cursor MainCursor => _mainConsole.Cursor;

        public Console MainConsole => _mainConsole;

        public ScrollingConsole(int width, int height, int bufferHeight, Font font = null)
        {
            UseKeyboard = false;
            UseMouse = true;
            _controlsHost = new ControlsConsole(1, height);

            var borderSurface = new Basic(width + 2, height + 2, Font);
            borderSurface.DrawBox(new Rectangle(0, 0, borderSurface.Width, borderSurface.Height),
                new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            borderSurface.Position = new Point(-1, -1);
            Children.Add(borderSurface);

            _mainConsole = new Console(width - 1, bufferHeight)
            {
                ViewPort = new Rectangle(0, 0, width - 1, height),
                Cursor = {IsVisible = false},
                Font = font == null ? Font:font
            };

            _scrollBar = ScrollBar.Create(Orientation.Vertical, height);
            _scrollBar.IsEnabled = false;
            _scrollBar.ValueChanged += ScrollBar_ValueChanged;

            _controlsHost.Add(_scrollBar);
            _controlsHost.Position = new Point(1 + _mainConsole.Width, Position.Y);

            Children.Add(_mainConsole);
            Children.Add(_controlsHost);

            _scrollingCounter = 0;
        }

        public void Reset()
        {
            _mainConsole.Clear();
            _mainConsole.Cursor.Position = new Point(0,0);
            _scrollingCounter = 0;
            _scrollBar.Value = _scrollingCounter;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Set the visible area of the console based on where the scroll bar is
            _mainConsole.ViewPort = new Rectangle(0, _scrollBar.Value, _mainConsole.Width, _mainConsole.ViewPort.Height);
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            // Send keyboard input to the main console
            return _mainConsole.ProcessKeyboard(state);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Check the scroll bar for mouse info first. If mouse not handled by scroll bar, then..

            // Create a mouse state based on the controlsHost
            if (!_controlsHost.ProcessMouse(new MouseConsoleState(_controlsHost, state.Mouse)))
            {
                // Process this console normally.
                return _mainConsole.ProcessMouse(state);
            }

            return false;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // If we detect that this console has shifted the data up for any reason (like the virtual cursor reached the
            // bottom of the entire text surface, OR we reached the bottom of the render area, we need to adjust the
            // scroll bar and follow the cursor
            if (_mainConsole.TimesShiftedUp != 0 | _mainConsole.Cursor.Position.Y >= _mainConsole.ViewPort.Height + _scrollingCounter)
            {
                // Once the buffer has finally been filled enough to need scrolling (a single screen's worth), turn on the scroll bar
                _scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (_scrollingCounter < _mainConsole.Height - _mainConsole.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    _scrollingCounter += _mainConsole.TimesShiftedUp != 0 ? _mainConsole.TimesShiftedUp : 1;

                _scrollBar.Maximum = _mainConsole.Height + _scrollingCounter - _mainConsole.Height;

                // This will follow the cursor since we move the render area in the event.
                _scrollBar.Value = _scrollingCounter;

                // Reset the shift amount.
                _mainConsole.TimesShiftedUp = 0;
            }
        }
    }
}
