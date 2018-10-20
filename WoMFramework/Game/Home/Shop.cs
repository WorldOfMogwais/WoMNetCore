using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Troschuetz.Random;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Home
{
    public class Shop
    {
        private Shift _currentShift;
        private IGenerator _randomGenerator;
        private double _gold;

        public List<BaseItem> Inventory;

        public Shop(Shift shift)
        {
            _gold = 50;
            Inventory = new List<BaseItem>();
            Resupply(shift);
        }

        public void Resupply(Shift shift)
        {
            foreach (var item in Inventory)
            {
                _gold += item.Cost;
            }

            Inventory.Clear();

            _currentShift = shift;
            _randomGenerator = new Dice(shift, 5804).GetRandomGenerator();

            List<BaseItem> allItems = new List<BaseItem>();
            allItems.AddRange(Weapons.Instance.All());
            allItems.AddRange(Armors.Instance.All());

            var potentialShopItems = allItems.Where(p => p.Cost < (_gold / 2)).ToList();

            int count = 0;
            while (potentialShopItems.Any())
            {
                // make sure at least one of each ... item category.
                var filteredPotentialShopItems = 
                    count == 0 ? potentialShopItems.OfType<Armor>().ToList<BaseItem>() :
                    count == 1 ? potentialShopItems.OfType<Weapon>().ToList<BaseItem>() : 
                    potentialShopItems;
                var shopItem = ChooseRandomItem(filteredPotentialShopItems);
                potentialShopItems.Remove(shopItem);
                _gold -= shopItem.Cost;
                Inventory.Add(shopItem);
                potentialShopItems = potentialShopItems.Where(p => p.Cost < _gold).ToList();
                count++;
            }
        }

        private BaseItem ChooseRandomItem(List<BaseItem> potentialShopItems)
        {
            if (potentialShopItems.Count == 0)
            {
                return null;
            }
            return potentialShopItems[_randomGenerator.Next(potentialShopItems.Count())];
        }
    }
}
