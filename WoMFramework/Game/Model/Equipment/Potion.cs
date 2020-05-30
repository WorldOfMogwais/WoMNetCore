namespace WoMFramework.Game.Model
{
    using Learnable;

    public sealed class PotionBuilder
    {
        public string Name { get; set; }
        public double Cost { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public RarityType RarityType { get; set; }
        public Spell Spell { get; set; }

        public Potion Build()
        {
            return new Potion(Name, Cost, Weight, Description, RarityType, Spell);
        }
    }

    public class Potion : BaseItem
    {
        public Spell Spell { get; }

        public Potion(string name, double cost, double weight, string description, RarityType rarityType, Spell spell) : base(name, cost, weight, description, rarityType)
        {
            Spell = spell;
        }
    }

    public class Potions
    {
        public static Potion CureLightWoundsPotion => new Potion("Cure Light Wounds", 45, 0.1, "When laying your hand upon a living creature, you channel positive energy that cures 1d8 points of damage +1 point per caster level (maximum +5).", RarityType.Magic, Spells.CureLightWounds());
    }
}
