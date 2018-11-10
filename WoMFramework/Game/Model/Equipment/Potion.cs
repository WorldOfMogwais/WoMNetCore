using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Spells;

namespace WoMFramework.Game.Model
{
    public sealed class PotionBuilder
    {
        public string Name  { get; set; }
        public double Cost { get; set; }
        public double Weight { get; set; }
        public string Description = string.Empty;

        public Spell Spell { get; set; }

        public Potion Build()
        {
            return new Potion(Name, Cost, Weight, Description, Spell);
        }
    }

    public class Potion : BaseItem
    {
        public Potion(string name, double cost, double weight, string description, Spell spell) : base(name, cost, weight, description)
        {

        }
    }
}
