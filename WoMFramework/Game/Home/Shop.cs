using System;
using System.Collections.Generic;
using System.Text;
using WoMFramework.Game.Interaction;

namespace WoMFramework.Game.Home
{
    public class Shop
    {
        private Shift _currentSupplyShift;

        public Shop(Shift shift)
        {
            Resupply(shift);
        }

        public void Resupply(Shift shift)
        {
            _currentSupplyShift = shift;
        }
    }
}
