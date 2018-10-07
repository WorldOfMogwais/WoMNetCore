using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Equipment;

namespace WoMFramework.Game.Model.Actions
{
    public abstract class EntityAction
    {
        public Entity Owner { get; }

        protected EntityAction(Entity owner)
        {
            Owner = owner;
        }

        public bool IsExecutable { get; set; }
    }

    public enum ActionType
    {
        None,
        Standard,
        Move,
        Full,
        Swift,
        Free,
        Immediate
    }

    public abstract class CombatAction : EntityAction
    {
        public ActionType ActionType { get; }

        public bool ProvokesAttackOfOpportunity;

        public Entity Target { get; }

        protected CombatAction(ActionType actionType, Entity owner, Entity target, bool provokesAttackOfOpportunity) : base(owner)
        {
            ActionType = actionType;
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
            Target = target;
        }

        public static CombatAction CreateStandardAction(Entity owner, Weapon weapon)
        {
            switch (weapon.WeaponEffortType)
            {
                case WeaponEffortType.Unarmed:
                    return new UnarmedAttack(owner, weapon);
                case WeaponEffortType.Light:
                    return new MeleeAttack(owner, weapon);
                case WeaponEffortType.OneHanded:
                    return new MeleeAttack(owner, weapon);
                case WeaponEffortType.TwoHanded:
                    return new MeleeAttack(owner, weapon);
                case WeaponEffortType.Ranged:
                    return new RangedAttack(owner, weapon);
                case WeaponEffortType.Ammunition:
                case WeaponEffortType.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract CombatAction Executable(Entity target);
    }

    public abstract class WeaponAttack : CombatAction
    {
        public Weapon Weapon { get; }

        protected WeaponAttack(ActionType actionType, Entity owner, Entity target, Weapon weapon, bool provokesAttackOfOpportunity) : base(actionType, owner,target, provokesAttackOfOpportunity)
        {
            Weapon = weapon;
        }

        public virtual int GetRange()
        {
            return Weapon.Range / 5 + 1;
        }

        public virtual bool IsInRange()
        {
            if (Target == null)
                return false;

            // TODO: range check

            return true;
        }

        public virtual bool IsInLos()
        {
            if (Target == null)
                return false;

            // TODO: line of sight check.

            return true;
        }
    }

    public class UnarmedAttack : WeaponAttack
    {
        public UnarmedAttack(Entity owner, Weapon weapon) : base(ActionType.Standard, owner, null, weapon, true)
        {
            IsExecutable = false;
        }
        private UnarmedAttack(Entity owner, Weapon weapon, Entity target) : base(ActionType.Standard, owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(Entity target)
        {
            // TODO: do all necessary checks here !!!
            return new UnarmedAttack(Owner, Weapon, target);
        }
    }

    public class MeleeAttack : WeaponAttack
    {
        public MeleeAttack(Entity owner, Weapon weapon) : base(ActionType.Standard, owner, null, weapon, false)
        {
            IsExecutable = false;
        }
        private MeleeAttack(Entity owner, Weapon weapon, Entity target) : base(ActionType.Standard, owner, target, weapon, false)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(Entity target)
        {
            // TODO: do all necessary checks here !!!
            return new MeleeAttack(Owner, Weapon, target);
        }
    }

    public class RangedAttack : WeaponAttack
    {
        public RangedAttack(Entity owner, Weapon weapon) : base(ActionType.Standard, owner, null, weapon, true)
        {
            IsExecutable = false;
        }
        private RangedAttack(Entity owner, Weapon weapon, Entity target) : base(ActionType.Standard, owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(Entity target)
        {
            // TODO: do all necessary checks here !!!
            return new RangedAttack(Owner, Weapon, target);
        }
    }

    public class SpellCast : CombatAction
    {
        public SpellCast(Entity owner) : base(ActionType.Standard, owner, null, false)
        {
            IsExecutable = false;
        }

        public override CombatAction Executable(Entity target)
        {
            return null;
        }
    }

    public class MoveAction : CombatAction
    {
        public MoveAction(Entity owner, bool provokesAttackOfOpportunity) : base(ActionType.Move, owner, null, provokesAttackOfOpportunity)
        {
        }

        public override CombatAction Executable(Entity target = null)
        {
            return null;
        }
    }

    public class FullMeleeAttack : WeaponAttack
    {
        public FullMeleeAttack(Entity owner, Weapon weapon) : base(ActionType.Full, owner, null, weapon, false)
        {
            IsExecutable = false;
        }
        private FullMeleeAttack(Entity owner, Weapon weapon, Entity target) : base(ActionType.Full, owner, target, weapon, false)
        {
            IsExecutable = true;
        }
        public override CombatAction Executable(Entity target)
        {
            // TODO: do all necessary checks here !!!
            return new FullMeleeAttack(Owner, Weapon, target);
        }
    }

    public class SwiftAction : CombatAction
    {
        public SwiftAction(Entity owner) : base(ActionType.Swift, owner, null, false)
        {
        }

        public override CombatAction Executable(Entity target = null)
        {
            return null;
        }
    }

    public class ImmediateAction : CombatAction
    {
        public ImmediateAction(Entity owner) : base(ActionType.Immediate, owner, null, false)
        {
        }

        public override CombatAction Executable(Entity target = null)
        {
            return null;
        }
    }

    public class FreeAction : CombatAction
    {
        public FreeAction(Entity owner) : base(ActionType.Free, owner, null, false)
        {
        }

        public override CombatAction Executable(Entity target = null)
        {
            return null;
        }
    }

}
