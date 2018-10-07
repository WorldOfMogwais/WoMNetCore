using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model.Equipment
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
        public Armor Armor { get; set; }

        public int ArmorBonus => 0 + (Armor?.ArmorBonus ?? 0);

        public int ShieldBonus => 0;

        public List<WeaponSlot> WeaponSlots { get; }

        public List<Armor> Armors { get; }

        public List<EquipmentSlot> Slots { get; }

        public List<BaseItem> Inventory { get; }

        public Equipment()
        {
            WeaponSlots = new List<WeaponSlot>();
            Armors = new List<Armor>();
            Slots = new List<EquipmentSlot>();
            Inventory = new List<BaseItem>();
        }

        public void CreateEquipmentSlots(SlotType[] slotTypes)
        {
            foreach (var slotType in slotTypes)
            {
                Slots.Add(new EquipmentSlot(slotType));
            }
        }

        public bool CanEquipe(SlotType slotType, BaseItem baseItem, out EquipmentSlot slot)
        {
            slot = Slots.FirstOrDefault(p => p.SlotType == slotType);

            return Inventory.Contains(baseItem) 
                && baseItem.SlotType == slotType 
                && slot != null;
        }

        public bool Equip(SlotType slotType, BaseItem baseItem)
        {
            if (!CanEquipe(slotType, baseItem, out var slot))
            {
                return false;
            }

            // removed old item
            if (slot.BasicItem != null)
            {
                var oldItem = slot.BasicItem;
                slot.BasicItem = null;
                Inventory.Add(oldItem);
            }

            // add new item
            Inventory.Remove(baseItem);
            slot.BasicItem = baseItem;

            return true;
        }
    }
}