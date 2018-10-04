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

    public abstract class CombatAction : EntityAction
    {
        public bool ProvokesAttackOfOpportunity;

        protected CombatAction(Entity owner, bool provokesAttackOfOpportunity) : base(owner)
        {
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
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

        public abstract bool CanExecute(Entity attacker, Entity target, out CombatAction standard);
    }

    public abstract class StandardAction : CombatAction
    {
        public Entity Target { get; }

        protected StandardAction(Entity owner, Entity target, bool provokesAttackOfOpportunity) : base(owner, provokesAttackOfOpportunity)
        {
            Target = target;
        }
    }

    public abstract class WeaponAttack : StandardAction
    {
        public Weapon Weapon { get; }

        protected WeaponAttack(Entity owner, Weapon weapon, bool provokesAttackOfOpportunity) : base(owner, null, provokesAttackOfOpportunity)
        {
            Weapon = weapon;
        }

        protected WeaponAttack(Entity owner, Entity target, Weapon weapon, bool provokesAttackOfOpportunity) : base(owner,target, provokesAttackOfOpportunity)
        {
            Weapon = weapon;
        }

        public virtual int GetRange()
        {
            return Weapon.Range;
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
        public UnarmedAttack(Entity owner, Weapon weapon) : base(owner, weapon, true)
        {
            IsExecutable = false;
        }
        private UnarmedAttack(Entity owner, Weapon weapon, Entity target) : base(owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new UnarmedAttack(Owner, Weapon, target);
            return true;
        }
    }

    public class MeleeAttack : WeaponAttack
    {
        public MeleeAttack(Entity owner, Weapon weapon) : base(owner, weapon, false)
        {
            IsExecutable = false;
        }
        private MeleeAttack(Entity owner, Weapon weapon, Entity target) : base(owner, target, weapon, false)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new MeleeAttack(Owner, Weapon, target);
            return true;
        }
    }

    public class RangedAttack : WeaponAttack
    {
        public RangedAttack(Entity owner, Weapon weapon) : base(owner, weapon, true)
        {
            IsExecutable = false;
        }
        private RangedAttack(Entity owner, Weapon weapon, Entity target) : base(owner, target, weapon, true)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new RangedAttack(Owner, Weapon, target);
            return true;
        }
    }

    public class SpellCast : StandardAction
    {
        public SpellCast(Entity owner) : base(owner, null, false)
        {
            IsExecutable = false;
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

    public class MoveAction : CombatAction
    {
        public MoveAction(Entity owner, bool provokesAttackOfOpportunity) : base(owner, provokesAttackOfOpportunity)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

    public abstract class FullRoundAction : CombatAction
    {
        public FullRoundAction(Entity owner, bool provokesAttackOfOpportunity) : base(owner, provokesAttackOfOpportunity)
        {
        }
    }

    public class FullAttack : FullRoundAction
    {
        public FullAttack(Entity owner) : base(owner, false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

    public class SwiftAction : CombatAction
    {
        public SwiftAction(Entity owner) : base(owner, false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;        }
    }

    public class ImmediateAction : CombatAction
    {
        public ImmediateAction(Entity owner) :base(owner, false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

    public class FreeAction : CombatAction
    {
        public FreeAction(Entity owner) : base(owner, false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

}
