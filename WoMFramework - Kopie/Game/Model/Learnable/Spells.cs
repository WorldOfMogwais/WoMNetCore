using System.Collections.Generic;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model
{
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
                DescriptorTypes = new DescriptorType[] {},
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
                    var owner = m as Entity;
                    var target = t as Entity;

                    if (owner == null || target == null)
                        return;

                    var casterLevel = owner.GetRequirementValue(RequirementType.CasterLevel);
                    var healAmount = owner.Dice.Roll(new int[] {1, 8, 0, (casterLevel < 5 ? casterLevel : 5)});
                    target.Heal(healAmount, HealType.Spell);
                })
            };
        }



    }



}
