using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using System;
using System.Threading.Tasks;
using WoMWallet.Node;

namespace WoMSadGui
{
    internal class MogwaiProgressDialog : Window
    {
        public Button button;
        private ProgressBar progressbar;
        private MogwaiController controller;

        private Progress<float> progressIndicator;

        public bool IsComplete { get; set; } = false;

        public MogwaiProgressDialog(string title, string text, MogwaiController mogwaicontroller, int width, int height) : base(width, height)
        {
            Title = "[" + title + "]";
            base.Fill(Color.DarkCyan, Color.Black, null);

            var label1 = new DrawingSurface(Width - 4, height - 5)
            {
                Position = new Point(2, 2)
            };
            label1.Surface.Fill(Color.Cyan, Color.Black, null);
            label1.Surface.Print(0, 0, text, Color.DarkCyan);
            Add(label1);

            progressbar = new ProgressBar(Width - 4, 1, HorizontalAlignment.Left);
            progressbar.Position = new Point(2, 4);
            Add(progressbar);
            Center();

            controller = mogwaicontroller;

            progressIndicator = new Progress<float>(UpdateProgressBar);
        }

        public void AddButon(string text)
        {
            button = new Button(text.Length + 2, 1);
            button.Position = new Point((Width - text.Length) / 2, 6);
            button.Text = text;
            Add(button);
        }

        private void UpdateProgressBar(float value)
        {
            progressbar.Progress = value;
            var tt = (int)(value * 100);
            button.Text = tt < 100 ? tt.ToString("#0") : "ok";
        }

        public async void StartAsync()
        {
            await Blockchain.Instance.CacheBlockhashesAsync(progressIndicator);
            IsComplete = true;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);
        }
    }
}