﻿namespace WoMFramework.Game.Home
{
    using Interaction;
    using Model;
    using Random;
    using System.Collections.Generic;
    using System.Linq;
    using Troschuetz.Random;

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
            foreach (BaseItem item in Inventory)
            {
                _gold += item.Cost;
            }

            Inventory.Clear();

            _currentShift = shift;
            _randomGenerator = new Dice(shift, 5804).GetRandomGenerator();

            var allItems = new List<BaseItem>();
            allItems.AddRange(Weapons.Instance.AllBuilders().Select(p => p.Build()));
            allItems.AddRange(Armors.Instance.AllBuilders().Select(p => p.Build()));

            var potentialShopItems = allItems.Where(p => p.Cost < (_gold / 2)).ToList();

            var count = 0;
            while (potentialShopItems.Any())
            {
                // make sure at least one of each ... item category.
                List<BaseItem> filteredPotentialShopItems =
                    count == 0 ? potentialShopItems.OfType<Armor>().ToList<BaseItem>() :
                    count == 1 ? potentialShopItems.OfType<Weapon>().ToList<BaseItem>() :
                    potentialShopItems;
                BaseItem shopItem = ChooseRandomItem(filteredPotentialShopItems);
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

            return potentialShopItems[_randomGenerator.Next(potentialShopItems.Count)];
        }
    }
}
