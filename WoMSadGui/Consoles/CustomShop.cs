namespace WoMSadGui.Consoles
{
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Controls;
    using Specific;
    using WoMFramework.Game.Model;
    using WoMFramework.Game.Model.Mogwai;
    using Coloring = Specific.Coloring;
    using Console = SadConsole.Console;

    internal class CustomShop : MogwaiConsole
    {
        private readonly Mogwai _mogwai;

        private readonly ControlsConsole _controlsConsole;

        private readonly Console _itemConsole;

        private MogwaiListBox _listBox;

        public CustomShop(Mogwai mogwai, int width, int height) : base("Home Town Shop", "", width, height)
        {
            _mogwai = mogwai;
            Fill(DefaultForeground, new Color(100, 0, 200, 150), null);

            _controlsConsole = new ControlsConsole(26, 20) { Position = new Point(1, 1) };
            _controlsConsole.Fill(Color.Transparent, new Color(100, 0, 200, 150), null);
            Children.Add(_controlsConsole);

            _itemConsole = new MogwaiConsole("Item", "", 50, 20) { Position = new Point(30, 1) };
            //_itemConsole.Fill(Color.Transparent, Color.DarkKhaki, null);
            Children.Add(_itemConsole);

            Init();
        }

        public void Init()
        {
            _listBox = new MogwaiListBox(26, 12)
            {
                Position = new Point(0, 0),
                HideBorder = false
            };
            foreach (BaseItem baseItem in _mogwai.HomeTown.Shop.Inventory)
            {
                _listBox.Items.Add(baseItem);
            }

            _controlsConsole.Add(_listBox);
            _listBox.SelectedItemChanged += _listBox_SelectedItemChanged;

            var btnBuy = new Button(8, 1) { Text = "BUY", Position = new Point(1, 15) };

            _controlsConsole.Add(btnBuy);
        }

        private void _listBox_SelectedItemChanged(object sender, ListBox.SelectedItemEventArgs e)
        {
            _itemConsole.Clear();

            if (e.Item is BaseItem baseItem)
            {
                _itemConsole.Print(1, 1,
                    $"{Coloring.Orange(baseItem.Name)} [{Coloring.DarkGrey(baseItem.GetType().Name)}]");
                _itemConsole.Print(1, 2,
                    $"Weight: {Coloring.Gainsboro(baseItem.Weight.ToString("######0.00").PadLeft(10))} lbs.",
                    Color.Gainsboro);
                _itemConsole.Print(1, 3,
                    $"Cost:   {Coloring.Gold(baseItem.Cost.ToString("######0.00").PadLeft(10))} Gold", Color.Gainsboro);
                if (baseItem is Weapon weapon)
                {
                    _itemConsole.Print(1, 5,
                        $"{Coloring.DarkGrey("Damage:")}   {Coloring.Green(weapon.MinDmg.ToString())} - {Coloring.Green(weapon.MaxDmg.ToString())}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 6,
                        $"{Coloring.DarkGrey("Critical:")} {Coloring.Yellow(weapon.CriticalMinRoll)} / {Coloring.Yellow(weapon.CriticalMultiplier)}x ",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 7, $"{Coloring.DarkGrey("Range:")}    {weapon.Range}", Color.Gainsboro);
                    _itemConsole.Print(1, 8,
                        $"{Coloring.DarkGrey("Damage:")}   {string.Join(",", weapon.WeaponDamageTypes)}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 9, $"{Coloring.DarkGrey("Effort:")}   {weapon.WeaponEffortType}",
                        Color.Gainsboro);
                }
                else if (baseItem is Armor armor)
                {
                    _itemConsole.Print(1, 5,
                        $"{Coloring.DarkGrey("Armor:")}    {Coloring.Green(armor.ArmorBonus.ToString())}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 6,
                        $"{Coloring.DarkGrey("Penalty:")} {Coloring.Red(armor.ArmorCheckPenalty.ToString())}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 7, $"{Coloring.DarkGrey("Max Dex:")}  {armor.MaxDexterityBonus}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 8, $"{Coloring.DarkGrey("Spellf.:")}  {armor.ArcaneSpellFailureChance}",
                        Color.Gainsboro);
                    _itemConsole.Print(1, 9, $"{Coloring.DarkGrey("Effort:")}   {armor.ArmorEffortType}",
                        Color.Gainsboro);
                }
            }

            //_itemConsole.Print(1, 15, $"{baseItem.Description}", Color.Gainsboro);
        }

        private void DoAction(string actionStr)
        {
        }
    }
}
