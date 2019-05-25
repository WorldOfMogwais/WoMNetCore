using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{
    public class EquipmentSlot
    {
        public SlotType SlotType { get; }
        public BaseItem BasicItem { get; set; }

        public EquipmentSlot(SlotType slotType)
        {
            SlotType = slotType;
        }
    }

    public class WeaponSlot
    {
        public bool IsEmpty => PrimaryWeapon == null && SecondaryWeapon == null;
        public Weapon PrimaryWeapon { get; set; }
        public Weapon SecondaryWeapon { get; set; }
    }

    public class Equipment
    {
        public int ArmorBonus => 0 + Slots.Select(p => p.BasicItem).OfType<Armor>().Sum(p => p.ArmorBonus);

        public int ShieldBonus => 0;

        public List<WeaponSlot> WeaponSlots { get; }

        public List<EquipmentSlot> Slots { get; }

        public Equipment()
        {
            WeaponSlots = new List<WeaponSlot>();
            Slots = new List<EquipmentSlot>();
        }

        public void CreateEquipmentSlots(SlotType[] slotTypes)
        {
            foreach (var slotType in slotTypes)
            {
                Slots.Add(new EquipmentSlot(slotType));
            }
        }

        public BaseItem GetItemInSlot(SlotType slotType) => Slots.FirstOrDefault(p => p.SlotType == slotType)?.BasicItem;

        public WeaponSlot GetWeaponSlot(int slotIndex)
        {
            return WeaponSlots.Count <= slotIndex ? null : WeaponSlots[slotIndex];
        }
    }
}