using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Input;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    class ScrollingConsole : SadConsole.ConsoleContainer
    {
        SadConsole.Console mainConsole;                 // The main console that is typed into
        SadConsole.ControlsConsole controlsHost;        // The scroll bar host
        SadConsole.Controls.ScrollBar scrollBar;        // The scroll bar

        int scrollingCounter;   // This is a counter to indicate how much buffer is used

        public Cursor MainCursor => mainConsole.Cursor; 

        public ScrollingConsole(int width, int height, int bufferHeight)
        {
            UseKeyboard = false;
            UseMouse = true;
            controlsHost = new ControlsConsole(1, height);

            mainConsole = new Console(width - 1, bufferHeight);
            mainConsole.ViewPort = new Rectangle(0, 0, width - 1, height);
            mainConsole.Cursor.IsVisible = false;

            scrollBar = SadConsole.Controls.ScrollBar.Create(SadConsole.Orientation.Vertical, height);
            scrollBar.IsEnabled = false;
            scrollBar.ValueChanged += ScrollBar_ValueChanged;

            controlsHost.Add(scrollBar);
            controlsHost.Position = new Point(1 + mainConsole.Width, Position.Y);

            Children.Add(mainConsole);
            Children.Add(controlsHost);

            scrollingCounter = 0;
        }

        private void ScrollBar_ValueChanged(object sender, EventArgs e)
        {
            // Set the visible area of the console based on where the scroll bar is
            mainConsole.ViewPort = new Rectangle(0, scrollBar.Value, mainConsole.Width, mainConsole.ViewPort.Height);
        }

        public override bool ProcessKeyboard(Keyboard state)
        {
            // Send keyboard input to the main console
            return mainConsole.ProcessKeyboard(state);
        }

        public override bool ProcessMouse(MouseConsoleState state)
        {
            // Check the scroll bar for mouse info first. If mouse not handled by scroll bar, then..

            // Create a mouse state based on the controlsHost
            if (!controlsHost.ProcessMouse(new MouseConsoleState(controlsHost, state.Mouse)))
            {
                // Process this console normally.
                return mainConsole.ProcessMouse(state);
            }

            return false;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            // If we detect that this console has shifted the data up for any reason (like the virtual cursor reached the
            // bottom of the entire text surface, OR we reached the bottom of the render area, we need to adjust the 
            // scroll bar and follow the cursor
            if (mainConsole.TimesShiftedUp != 0 | mainConsole.Cursor.Position.Y >= mainConsole.ViewPort.Height + scrollingCounter)
            {
                // Once the buffer has finally been filled enough to need scrolling (a single screen's worth), turn on the scroll bar
                scrollBar.IsEnabled = true;

                // Make sure we've never scrolled the entire size of the buffer
                if (scrollingCounter < mainConsole.Height - mainConsole.ViewPort.Height)
                    // Record how much we've scrolled to enable how far back the bar can see
                    scrollingCounter += mainConsole.TimesShiftedUp != 0 ? mainConsole.TimesShiftedUp : 1;

                scrollBar.Maximum = (mainConsole.Height + scrollingCounter) - mainConsole.Height;

                // This will follow the cursor since we move the render area in the event.
                scrollBar.Value = scrollingCounter;

                // Reset the shift amount.
                mainConsole.TimesShiftedUp = 0;
            }
        }
    }
}
