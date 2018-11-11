using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Model.Actions;

namespace WoMFramework.Game.Model.Spells
{
    public enum SchoolType
    {
        None,
        Conjuration,
    }

    public enum SubSchoolType
    {
        None,
        Creation,
        Healing
    }

    public enum DescriptorType
    {
        None,
        Acid
    }

    public enum RangeType
    {
        None,
        Touch,
    }

    public enum AreaType
    {
        None
    }

    public enum EffectType
    {
        None
    }

    public enum TargetType
    {
        None,
        Entity
    }

    public enum DurationType
    {
        None,
        Instant
    }

    public enum SavingThrowType
    {
        None,
        Will,
        Reflex,
        Fortitude
    }

    public class Spell
    {
        public int Id { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription { get; set; }

        public SchoolType SchoolType { get; set; }

        public SubSchoolType SubSchoolType { get; set; }

        public DescriptorType[] DescriptorTypes { get; set; }

        public List<Requirement> Requirements { get; set; }

        public ActionType CastingTime { get; set; }

        public RangeType RangeType { get; set; }

        public AreaType AreaType { get; set; }

        public EffectType EffectType { get; set; }
        
        public TargetType TargetType { get; set; }

        public DurationType DurationType { get; set; }

        public SavingThrowType SavingThrowType { get; set; }

        public double SpellResistance { get; set; }

        public Action<IAdventureEntity, IAdventureEntity> SpellEffect { get; set; }

        public Spell(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool CanExecuteSpell(IAdventureEntity me, IAdventureEntity target = null)
        {
            return true;
        }

        public virtual bool ExecuteSpell(IAdventureEntity me, IAdventureEntity target = null)
        {
            if (!CanExecuteSpell(me, target))
            {
                return false;
            }

            SpellEffect(me, target);
            return true;
        }
    }

    public class Spells
    {
        public static Spell CureLightWounds()
        {
            return new Spell(0, "Cure Light Wounds")
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
                    var healAmount = owner.Dice.Roll(8) + casterLevel < 5 ? casterLevel : 5;
                    target.Heal(healAmount, HealType.Spell);
                })
            };
        }



    }



}
