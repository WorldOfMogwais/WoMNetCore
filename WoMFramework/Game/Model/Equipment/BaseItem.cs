using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public class BaseItem
    {
        public SlotType SlotType { get; }
        public string Name { get; }
        public double Cost { get; }
        public double Weight { get; }
        public string Description { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cost"></param>
        /// <param name="weight"></param>
        /// <param name="description"></param>
        /// <param name="slotType"></param>
        public BaseItem(string name, double cost, double weight, string description, SlotType slotType = SlotType.None)
        {
            SlotType = slotType;
            Name = name;
            Cost = cost;
            Weight = weight;
            Description = description;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
