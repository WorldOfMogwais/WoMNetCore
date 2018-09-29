namespace WoMFramework.Game.Model.Equipment
{
    public class BaseItem
    {
        public string Name { get; }
        public double Cost { get; }
        public double Weight { get; }
        public string Description { get; set; }

        public BaseItem(string name, double cost, double weight, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Description = description;
        }
    }
}
