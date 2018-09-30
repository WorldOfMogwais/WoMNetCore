using System.Collections.Generic;

namespace WoMFramework.Game.Model.Equipment
{
    public class Equipment
    {
        public Weapon BaseWeapon { get; set; }

        private Weapon _primaryWeapon;
        public Weapon PrimaryWeapon {
            get => _primaryWeapon ?? BaseWeapon;
            set => _primaryWeapon = value;
        }

        public Weapon SecondaryWeapon { get; set; }

        public Armor Armor { get; set; }

        public int ArmorBonus => 0 + (Armor?.ArmorBonus ?? 0);

        public int ShieldBonus => 0;

        public List<Weapon> Weapons { get; set; }

        public List<Armor> Armors { get; set; }

        public Equipment()
        {
            Weapons = new List<Weapon>();
            Armors = new List<Armor>();
        }
    }
}