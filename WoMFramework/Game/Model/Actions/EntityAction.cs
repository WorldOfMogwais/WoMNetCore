using System;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator;

namespace WoMFramework.Game.Model.Actions
{
    public abstract class EntityAction
    {
        public Entity Owner { get; private set; }

        protected EntityAction(Entity owner)
        {
            Owner = owner;
        }

        public bool IsExecutable { get; set; }

        public void ChangeOwner(Entity entity)
        {
            Owner = entity;
        }
    }

    public enum ActionType
    {
        None,
        Immediate,
        Free,
        Swift,
        Move,
        Standard,
        Full
    }

    public abstract class CombatAction : EntityAction
    {
        public ActionType ActionType { get; }

        public bool ProvokesAttackOfOpportunity;

        public AdventureEntity Target { get; }

        protected CombatAction(ActionType actionType, Entity owner, AdventureEntity target, bool provokesAttackOfOpportunity) : base(owner)
        {
            ActionType = actionType;
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
            Target = target;
        }

        public static CombatAction CreateWeaponAttack(Entity owner, Weapon weapon, bool fullRound)
        {
            switch (weapon.WeaponEffortType)
            {
                case WeaponEffortType.Unarmed:
                    return new UnarmedAttack(owner, weapon, fullRound);
                case WeaponEffortType.Light:
                    return new MeleeAttack(owner, weapon, fullRound);
                case WeaponEffortType.OneHanded:
                    return new MeleeAttack(owner, weapon, fullRound);
                case WeaponEffortType.TwoHanded:
                    return new MeleeAttack(owner, weapon, fullRound);
                case WeaponEffortType.Ranged:
                    return new RangedAttack(owner, weapon, fullRound);
                case WeaponEffortType.Ammunition:
                case WeaponEffortType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static CombatAction CreateMove(Entity owner)
        {
            return new Move(owner);
        }

        public abstract CombatAction Executable(AdventureEntity target);
    }

    public abstract class WeaponAttack : CombatAction
    {
        public Weapon Weapon { get; }

        protected WeaponAttack(ActionType actionType, Entity owner, AdventureEntity target, Weapon weapon, bool provokesAttackOfOpportunity) : base(actionType, owner, target, provokesAttackOfOpportunity)
        {
            Weapon = weapon;
        }

        public virtual int GetRange()
        {
            return Weapon.Range / 5 + 1;
        }

        public virtual bool InWeaponRange(AdventureEntity target)
        {
            if (Owner == null || target == null)
            {
                return false;
            }

            return Distance.EUCLIDEAN.Calculate(target.Coordinate - Owner.Coordinate) <= GetRange();
        }

    }

    public class UnarmedAttack : WeaponAttack
    {
        public UnarmedAttack(Entity owner, Weapon weapon, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, null, weapon, true)
        {
            IsExecutable = false;
        }
        private UnarmedAttack(Entity owner, Weapon weapon, AdventureEntity target, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(AdventureEntity target)
        {
            if (!InWeaponRange(target))
            {
                return null;
            }
            return new UnarmedAttack(Owner, Weapon, target, ActionType == ActionType.Full);
        }
    }

    public class MeleeAttack : WeaponAttack
    {
        public MeleeAttack(Entity owner, Weapon weapon, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, null, weapon, false)
        {
            IsExecutable = false;
        }
        private MeleeAttack(Entity owner, Weapon weapon, AdventureEntity target, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, target, weapon, false)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(AdventureEntity target)
        {
            if (!InWeaponRange(target))
            {
                return null;
            }
            return new MeleeAttack(Owner, Weapon, target, ActionType == ActionType.Full);
        }
    }

    public class RangedAttack : WeaponAttack
    {
        public RangedAttack(Entity owner, Weapon weapon, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, null, weapon, true)
        {
            IsExecutable = false;
        }
        private RangedAttack(Entity owner, Weapon weapon, AdventureEntity target, bool fullRound) : base(fullRound ? ActionType.Full : ActionType.Standard, owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(AdventureEntity target)
        {
            if (!InWeaponRange(target))
            {
                return null;
            }
            return new RangedAttack(Owner, Weapon, target, ActionType == ActionType.Full);
        }
    }

    public class DrinkPotion : CombatAction
    {
        public DrinkPotion(Entity owner) : base(ActionType.Standard, owner, null, true)
        {
            IsExecutable = false;
        }

        public override CombatAction Executable(AdventureEntity target)
        {
            return null;
        }
    }

    public class SpellCast : CombatAction
    {
        public Spell Spell { get; }

        public SpellCast(Entity owner, Spell spell, ActionType actionType) : base(actionType, owner, null, true)
        {
            IsExecutable = false;
            Spell = spell;
        }
        private SpellCast(Entity owner, Spell spell, AdventureEntity target, ActionType actionType) : base(actionType, owner, target, true)
        {
            IsExecutable = true;
            Spell = spell;
        }
        public override CombatAction Executable(AdventureEntity target)
        {
            if (!Spell.CanCast(Owner, target))
            {
                return null;
            }
            return new SpellCast(Owner, Spell, target, ActionType);
        }
    }

    public abstract class MoveAction : CombatAction
    {
        public Coord Destination => Target.Coordinate;

        protected MoveAction(Entity owner, AdventureEntity target, bool provokesAttacksofOpportunity) : base(ActionType.Move, owner, target, provokesAttacksofOpportunity)
        {
            IsExecutable = true;
        }

    }

    public class Move : MoveAction
    {
        public Move(Entity owner) : base(owner, null, true)
        {
            IsExecutable = false;
        }
        private Move(Entity owner, AdventureEntity target) : base(owner, target, true)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(AdventureEntity target)
        {
            return new Move(Owner, target);
        }
    }

    public class SwiftAction : CombatAction
    {
        public SwiftAction(Entity owner) : base(ActionType.Swift, owner, null, false)
        {
        }

        public override CombatAction Executable(AdventureEntity target)
        {
            return null;
        }
    }

    public class ImmediateAction : CombatAction
    {
        public ImmediateAction(Entity owner) : base(ActionType.Immediate, owner, null, false)
        {
        }

        public override CombatAction Executable(AdventureEntity target)
        {
            return null;
        }
    }

    public class FreeAction : CombatAction
    {
        public FreeAction(Entity owner) : base(ActionType.Free, owner, null, false)
        {
        }

        public override CombatAction Executable(AdventureEntity target)
        {
            return null;
        }
    }

}
