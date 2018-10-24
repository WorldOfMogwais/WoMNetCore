using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Controls;
using WoMFramework.Game.Model.Mogwai;
using WoMSadGui.Specific;

namespace WoMSadGui.Consoles
{
    internal class ShopConsole : MogwaiConsole
    {
        private readonly Mogwai _mogwai;

        private readonly ControlsConsole _controlsConsole;

        public ShopConsole(Mogwai mogwai, int width, int height) : base("Home Town Shop", "", width, height)
        {
            _controlsConsole = new ControlsConsole(50, 15) { Position = new Point(1, 1) };
            _controlsConsole.Fill(Color.DarkCyan, Color.Black, null);
            Children.Add(_controlsConsole);

            _mogwai = mogwai;
            Init();
        }

        public void Init()
        {
            var str = string.Join(';',_mogwai.HomeTown.Shop.Inventory.Select(p => p.Name).ToArray());

            var listbox = new MogwaiListBox(25, 10)
            {
                Position = new Point(0, 5),
                HideBorder = false
            };
            foreach (var baseItem in _mogwai.HomeTown.Shop.Inventory)
            {
                listbox.Items.Add(baseItem.Name);
            }
            _controlsConsole.Add(listbox);

            AddRadioButton(28, 11, "All", DoAction);
            AddRadioButton(28, 12, "Armor", DoAction);
            AddRadioButton(28, 13, "Weapon", DoAction);
            AddRadioButton(28, 14, "Potion", DoAction);
        }

        private void AddRadioButton(int x, int y, string text, Action<string> buttonClicked)
        {
            var radioButton = new MogwaiRadioButton(20, 1);
            radioButton.Text = text;
            radioButton.Position = new Point(x, y);
            _controlsConsole.Add(radioButton);
        }

        private void AddButton(int x, int y, string text, Action<string> buttonClicked)
        {
            var txt = text;
            var button = new MogwaiButton(6, 1)
            {
                Position = new Point(x, y),
                Text = txt
            };
            button.Click += (btn, args) =>
            {
                buttonClicked(((MogwaiButton)btn).Text);
            };
            _controlsConsole.Add(button);
        }

        private void DoAction(string actionStr)
        {
        }
    }
}