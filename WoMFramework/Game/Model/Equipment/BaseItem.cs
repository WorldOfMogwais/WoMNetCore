namespace WoMFramework.Game.Model.Equipment
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
