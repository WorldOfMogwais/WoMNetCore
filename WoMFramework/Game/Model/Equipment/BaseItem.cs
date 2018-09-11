using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMFramework.Game.Model
{
    public class BaseItem
    {
        public string Name { get; }
        public int Cost { get; }
        public double Weight { get; }
        public string Description { get; set; }

        public BaseItem(string name, int cost, double weight, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Description = description;
        }
    }
}
