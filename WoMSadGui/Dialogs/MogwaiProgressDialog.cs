using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using WoMWallet.Node;

namespace WoMSadGui.Dialogs
{
    internal class MogwaiProgressDialog : MogwaiDialog
    {
        private readonly ProgressBar _progressbar;

        private readonly Progress<float> _progressIndicator;

        public bool IsComplete { get; set; }

        public MogwaiProgressDialog(string title, string text, int width, int height) : base(title, text, width, height)
        {
            _progressbar = new ProgressBar(Width - 4, 1, HorizontalAlignment.Left) { Position = new Point(2, 4) };
            Add(_progressbar);

            _progressIndicator = new Progress<float>(UpdateProgressBar);
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