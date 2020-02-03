namespace WoMFramework.Game.Model.Learnable
{
    using Actions;
    using Enums;
    using System.Collections.Generic;

    public class Spells
    {
        public static Spell CureLightWounds()
        {
            return new Spell(0, "Cure Light Wounds", 1)
            {
                Description =
                    "When laying your hand upon a living creature, you channel positive energy that cures 1d8 points " +
                    "of damage + 1 point per caster level (maximum +5). Since undead are powered by negative energy, " +
                    "this spell deals damage to them instead of curing their wounds. An undead creature can apply spell " +
                    "resistance, and can attempt a Will save to take half damage.",
                ShortDescription = "Cures 1d8 damage + 1/level (max +5).",
                SchoolType = SchoolType.Conjuration,
                SubSchoolType = SubSchoolType.Healing,
                DescriptorTypes = new DescriptorType[] { },
                Requirements = new List<Requirement>(),
                CastingTime = ActionType.Standard,
                RangeType = RangeType.Touch,
                AreaType = AreaType.None,
                EffectType = EffectType.None,
                TargetType = TargetType.Entity,
                DurationType = DurationType.Instant,
                SavingThrowType = SavingThrowType.Will,
                SpellResistance = 0.5,
                SpellEffect = ((m, t) =>
                {
                    if (!(m is Entity owner) || !(t is Entity target))
                        return;

                    var casterLevel = owner.GetRequirementValue(RequirementType.CasterLevel);
                    var healAmount = owner.Dice.Roll(new int[] { 1, 8, 0, (casterLevel < 5 ? casterLevel : 5) });
                    target.Heal(healAmount, HealType.Spell);
                })
            };
        }

        public static Spell BurningHands()
        {
            return new Spell(1, "Burning Hands", 1)
            {
                Description =
                    "A cone of searing flame shoots from your fingertips. Any creature in the area of the flames " +
                    "takes 1d4 points of fire damage per caster level (maximum 5d4). Flammable materials burn if the " +
                    "flames touch them. A character can extinguish burning items as a full-round action.",
                ShortDescription = " 1d4/level fire damage (max 5d4).",
                SchoolType = SchoolType.Evocation,
                SubSchoolType = SubSchoolType.Injuring,
                DescriptorTypes = new[] { DescriptorType.Fire },
                Requirements = new List<Requirement>(),
                CastingTime = ActionType.Standard,
                RangeType = RangeType.Ft15,
                AreaType = AreaType.ConeShapedBurst,
                EffectType = EffectType.None,
                TargetType = TargetType.Entity,
                DurationType = DurationType.Instant,
                SavingThrowType = SavingThrowType.Reflex,
                SpellResistance = 0.5,
                SpellEffect = ((m, t) =>
                {
                    if (!(m is Entity owner) || !(t is Entity target))
                        return;

                    var casterLevel = owner.GetRequirementValue(RequirementType.CasterLevel);
                    var damageAmount = owner.Dice.Roll(new int[] { 1, 4, 0, (casterLevel < 5 ? casterLevel : 5) });
                    target.Damage(damageAmount, DamageType.Spell);
                })
            };
        }

        public static Spell RayOfFrost()
        {
            return new Spell(2, "Ray of Frost", 0)
            {
                Description =
                    "A ray of freezing air and ice projects from your pointing finger. " +
                    "You must succeed on a ranged touch attack with the ray to deal damage to a target. " +
                    "The ray deals 1d3 points of cold damage.",
                ShortDescription = "1d3 cold damage.",
                SchoolType = SchoolType.Evocation,
                SubSchoolType = SubSchoolType.Injuring,
                DescriptorTypes = new[] { DescriptorType.Cold },
                Requirements = new List<Requirement>(),
                CastingTime = ActionType.Standard,
                RangeType = RangeType.Ft25,
                AreaType = AreaType.None,
                EffectType = EffectType.Ray,
                TargetType = TargetType.Entity,
                DurationType = DurationType.Instant,
                SavingThrowType = SavingThrowType.None,
                SpellResistance = 0.5,
                SpellEffect = (m, t) =>
                 {
                     if (!(m is Entity owner) || !(t is Entity target))
                         return;

                     var damageAmount = owner.Dice.Roll(3);
                     target.Damage(damageAmount, DamageType.Spell);
                 }
            };
        }
    }
}
