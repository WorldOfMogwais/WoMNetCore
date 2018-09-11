using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using WoMWallet.Node;

namespace WoMSadGui
{
    internal class MogwaiProgressDialog : Window
    {
        public Button Button;
        private ProgressBar _progressbar;
        private MogwaiController _controller;

        private Progress<float> _progressIndicator;

        public bool IsComplete { get; set; } = false;

        public MogwaiProgressDialog(string title, string text, MogwaiController mogwaicontroller, int width, int height) : base(width, height)
        {
            Title = "[" + title + "]";
            Fill(Color.DarkCyan, Color.Black, null);

            var label1 = new DrawingSurface(Width - 4, height - 5)
            {
                Position = new Point(2, 2)
            };
            label1.Surface.Fill(Color.Cyan, Color.Black, null);
            label1.Surface.Print(0, 0, text, Color.DarkCyan);
            Add(label1);

            _progressbar = new ProgressBar(Width - 4, 1, HorizontalAlignment.Left);
            _progressbar.Position = new Point(2, 4);
            Add(_progressbar);
            Center();

            _controller = mogwaicontroller;

            _progressIndicator = new Progress<float>(UpdateProgressBar);
        }

        public void AddButon(string text)
        {
            Button = new Button(text.Length + 2, 1);
            Button.Position = new Point((Width - text.Length) / 2, 6);
            Button.Text = text;
            Add(Button);
        }

        private void UpdateProgressBar(float value)
        {
            _progressbar.Progress = value;
            var tt = (int)(value * 100);
            Button.Text = tt < 100 ? tt.ToString("#0") : "ok";
        }

        public async void StartAsync()
        {
            await Blockchain.Instance.CacheBlockhashesAsync(_progressIndicator);
            IsComplete = true;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
        }
    }
}