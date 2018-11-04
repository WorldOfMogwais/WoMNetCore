using System;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Model.Mogwai;
using Console = SadConsole.Console;

namespace WoMSadGui.Consoles
{
    public class PlayStatsConsole : Console
    {
        private readonly Mogwai _mogwai;

        private readonly Basic _borderSurface;

        public PlayStatsConsole(Mogwai mogwai, int width, int height) : base(width, height)
        {
            _mogwai = mogwai;

            _borderSurface = new Basic(width + 2, height + 2, Font);
            _borderSurface.DrawBox(new Rectangle(0, 0, _borderSurface.Width, _borderSurface.Height),
                new Cell(Color.DarkCyan, Color.Black), null, ConnectedLineThick);
            _borderSurface.Position = new Point(-1, -1);
            Children.Add(_borderSurface);

            Init();
            
        }


        public override void Update(TimeSpan delta)
        {
            if (IsVisible)
            {
                UpdateScreen();
            }

            base.Update(delta);
        }

        public void UpdateScreen()
        {
            var nameStr = $"{_mogwai.Name} .:{_mogwai.CurrentLevel}:.";
            nameStr = nameStr.PadLeft(22 + nameStr.Length / 2).PadRight(44);
            Print(0, 0, $"[c:g b:red:black:black:blue:{nameStr.Length}]" + nameStr, Color.Orange);

            for (var i = 0; i < 2 && i < _mogwai.Classes.Count; i++)
            {
                var classes = _mogwai.Classes[i];
                Print(31, i + 3, classes.Name.PadRight(10), Color.Orange);
                Print(43, i + 3, classes.ClassLevel.ToString(), Color.Gold);
            }

            _mogwai.CanLevelClass(out var levels);
            Print(43, 5, levels.ToString(), Color.Aqua);

            CreateHealthBar(16, 2, Color.DarkGreen, Color.LimeGreen, _mogwai.CurrentHitPoints, _mogwai.MaxHitPoints);
            var lastLevelXp = _mogwai.LevelUpXp(_mogwai.CurrentLevel - 1);
            CreateExperienceBar(31, 6, Color.DarkOrange, Color.Gold, _mogwai.Exp - lastLevelXp, _mogwai.XpToLevelUp - lastLevelXp);
            PrintStat(1, 3, "STR", _mogwai.Strength, _mogwai.StrengthMod, 0);
            PrintStat(1, 4, "DEX", _mogwai.Dexterity, _mogwai.DexterityMod, 0);
            PrintStat(1, 5, "CON", _mogwai.Constitution, _mogwai.ConstitutionMod, 0);
            PrintStat(1, 6, "INT", _mogwai.Inteligence, _mogwai.InteligenceMod, 0);
            PrintStat(1, 7, "WIS", _mogwai.Wisdom, _mogwai.WisdomMod, 0);
            PrintStat(1, 8, "CHA", _mogwai.Charisma, _mogwai.CharismaMod, 0);

            PrintStat(16, 6, "AC", _mogwai.ArmorClass, 0, 0);
            PrintStat(16, 7, "INI", _mogwai.Initiative, 0, 0);
            PrintStat(16, 8, "AB", _mogwai.AttackBonus(0), 0, 0);

            PrintWeapons(16, 10, _mogwai);

            PrintArmor(16, 13, _mogwai);

            PrintStat(1, 10, "FOR", _mogwai.Fortitude, 0, 0);
            PrintStat(1, 11, "REF", _mogwai.Reflex, 0, 0);
            PrintStat(1, 12, "WIL", _mogwai.Will, 0, 0);

            PrintWealth(31, 17, _mogwai);

        }

        public void Init()
        {
            var alligmentGender = $".:-| {_mogwai.Stats.MapAllignment()} {_mogwai.GenderStr} |-:.";
            alligmentGender = alligmentGender.PadLeft(22 + alligmentGender.Length / 2).PadRight(44);
            Print(0, 1, new ColoredString($"[c:g b:red:black:black:blue:{alligmentGender.Length}]" + alligmentGender));
            Print(31, 5, "Unspent", Color.Gainsboro);


            _borderSurface.Print(0, 3, "[c:sg 204:1] ", Color.DarkCyan);
            _borderSurface.Print(45, 3, "[c:sg 185:1] ", Color.DarkCyan);
            Print(0, 2, "[c:sg 205:44]" + "".PadRight(44), Color.DarkCyan);

            _borderSurface.Print(0, 10, "[c:sg 204:1] ", Color.DarkCyan);
            _borderSurface.Print(45, 10, "[c:sg 185:1] ", Color.DarkCyan);
            Print(0, 9, "[c:sg 205:44]" + "".PadRight(44), Color.DarkCyan);

            _borderSurface.Print(0, 14, "[c:sg 204:1] ", Color.DarkCyan);
            Print(0, 13, "[c:sg 205:14]" + "".PadRight(30), Color.DarkCyan);
            _borderSurface.Print(45, 13, "[c:sg 185:1] ", Color.DarkCyan);
            Print(15, 12, "[c:sg 205:30]" + "".PadRight(30), Color.DarkCyan);


            Print(15, 5, "[c:sg 205:14]" + "".PadRight(14), Color.DarkCyan);
            Print(30, 6, "[c:sg 205:14]" + "".PadRight(14), Color.DarkCyan);

            _borderSurface.Print(45, 0, "[c:sg 203:1] ", Color.DarkCyan);

            Print(30, 16, "[c:sg 205:14]" + "".PadRight(14), Color.DarkCyan);
            _borderSurface.SetGlyph(45, 17, 185, Color.DarkCyan);
            Print(30, 18, "[c:sg 205:14]" + "".PadRight(14), Color.DarkCyan);
            _borderSurface.SetGlyph(45, 19, 185, Color.DarkCyan);
            Print(15, 17, "[c:sg 205:15]" + "".PadRight(30), Color.DarkCyan);
            _borderSurface.Print(30, 18, "[c:sg 185:1] ", Color.DarkCyan);

            SetGlyph(14, 13, 185, Color.DarkCyan);
            SetGlyph(14, 14, 186, Color.DarkCyan);
            SetGlyph(14, 15, 186, Color.DarkCyan);
            SetGlyph(14, 16, 186, Color.DarkCyan);
            SetGlyph(14, 17, 200, Color.DarkCyan);

            SetGlyph(29, 18, 200, Color.DarkCyan);
            SetGlyph(29, 17, 186, Color.DarkCyan);
            SetGlyph(29, 16, 201, Color.DarkCyan);

            SetGlyph(14, 2, 203, Color.DarkCyan);
            SetGlyph(14, 9, 206, Color.DarkCyan);
            SetGlyph(29, 5, 185, Color.DarkCyan);
            SetGlyph(29, 2, 203, Color.DarkCyan);
            SetGlyph(14, 5, 204, Color.DarkCyan);
            SetGlyph(29, 6, 204, Color.DarkCyan);
            SetGlyph(44, 6, 185, Color.DarkCyan);

            //SetGlyph(14, 12, 204, Color.DarkCyan);


            SetGlyph(29, 9, 202, Color.DarkCyan);
            //SetGlyph(14, 13, 202, Color.DarkCyan);

            //Print(16, 9, "Weapon");

            //Print(16, 12, "Armor");

        }

        private void PrintWealth(int x, int y, Mogwai mogwai)
        {
            Print(x, y, mogwai.Wealth.Gold.ToString().PadLeft(7), Color.White);
            Print(x + 8, y, "Gold", Color.Yellow);
        }

        private void PrintWeapons(int x, int y, Mogwai mogwai)
        {
            if (mogwai.Equipment.WeaponSlots.Count < 1)
            {
                return;
            }

            var primary = mogwai.Equipment.WeaponSlots[0].PrimaryWeapon;
            Print(x, y, "P:", Color.Gainsboro);
            Print(x + 3, y, primary.Name, Color.Orange);
            Print(x + 18, y, new ColoredString($"[c:r f:limegreen]{primary.MinDmg} [c:r f:gainsboro]- [c:r f:limegreen]{primary.MaxDmg} [c:r f:gainsboro]DMG".PadLeft(11)));
            if (mogwai.Equipment.WeaponSlots[0].SecondaryWeapon == null)
            {
                Print(x, y+1, "S:", Color.Gainsboro);
                Print(x + 3, y + 1, "None", Color.Gray);
                return;
            }
            var secondary =  mogwai.Equipment.WeaponSlots[0].SecondaryWeapon;
            Print(x, y + 1, "S:", Color.Gainsboro);
            Print(x + 3, y + 1, secondary.Name, Color.Orange);
            Print(x + 18, y + 1, new ColoredString($"[c:r f:limegreen]{secondary.MinDmg} [c:r f:gainsboro]- [c:r f:limegreen]{secondary.MaxDmg} [c:r f:gainsboro]DMG".PadLeft(11)));
        }

        private void PrintArmor(int x, int y, Mogwai mogwai)
        {
            if (mogwai.Equipment.Armor == null)
            {
                return;
            }

            var armor = mogwai.Equipment.Armor;

            Print(x, y, "A:", Color.Gainsboro);
            Print(x + 3, y, armor.Name.Substring(0, 15), Color.Orange);
            Print(x + 20, y, new ColoredString($"[c:r f:limegreen]{armor.ArmorBonus} [c:r f:gainsboro]AC [c:r f:red]{armor.ArmorCheckPenalty}".PadLeft(11)));
        }

        public void CreateExperienceBar(int x, int y, Color low, Color full, double current, double max)
        {
            Print(x, y + 1, current.ToString().PadLeft(9), Color.Orange);
            Print(x + 10, y + 1, "XP", Color.Gainsboro);
            CreateBar(x, y + 2, low, full, (float)(current / max));
            SetGlyph(x + 13, y + 1, 186, Color.DarkCyan);
            SetGlyph(x + 13, y + 2, 186, Color.DarkCyan);
        }

        public void CreateHealthBar(int x, int y, Color low, Color full, int current, int max)
        {
            Print(x, y + 1, $"{current}/{max}".PadLeft(9), Color.Orange);
            Print(x + 10, y + 1, "HP", Color.Gainsboro);
            if (current >= 0)
            {
                CreateBar(x, y + 2, low, full, (float) current / max);
            }
            else
            {
                CreateBar(x, y + 2, Color.DarkRed, Color.Red, (float) Math.Abs(current < -10 ? -10 : current)  / 10);
            }

            SetGlyph(x + 13, y + 1, 186, Color.DarkCyan);
            SetGlyph(x + 13, y + 2, 186, Color.DarkCyan);
        }

        public void CreateBar(int x, int y, Color low, Color full, float perc)
        {
            Print(x, y, "[c:sg 176:12]" + "".PadRight(13), low);
            SetGlyph(x + 13, y, 186, Color.DarkCyan);
            var p = perc * 36;
            var z = (int)(p / 3);
            var t = (int)(p % 3);
            var g = 176 + t;
            Print(x, y, $"[c:sg 219:{z}]" + "".PadRight(z), full);
            if (t != 0)
            {
                SetGlyph(x + z, y, g, full);
            }
        }

        public void PrintStat(int x, int y, string name, int value, int? mod, int temp)
        {
            var xO = x;
            var modCol = mod > 0 ? Color.LimeGreen : mod < 0 ? Color.Red : Color.Gray;
            var modSig = mod > 0 ? "+" : "";
            Print(x, y, name.PadLeft(3), Color.Gainsboro);
            x += 4;
            Print(x, y, value.ToString("#0").PadLeft(2), Color.Orange);
            if (mod != null)
            {
                x += 3;
                Print(x, y, (modSig + mod).PadLeft(2), modCol);
            }

            x += 3;
            Print(x, y, temp.ToString().PadLeft(2), Color.Aqua);
            SetGlyph(xO + 13, y, 186, Color.DarkCyan);
        }
    }
}