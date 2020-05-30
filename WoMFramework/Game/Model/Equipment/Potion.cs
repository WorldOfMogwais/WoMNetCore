namespace WoMFramework.Game.Model
{
    using Learnable;

    public sealed class PotionBuilder
    {
        public string Name { get; set; }
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

    public class Potions
    {
        public static Potion CureLightWoundsPotion => new Potion("Cure Light Wounds", 45, 0.1, "When laying your hand upon a living creature, you channel positive energy that cures 1d8 points of damage +1 point per caster level (maximum +5).", Spells.CureLightWounds());
    }
}
