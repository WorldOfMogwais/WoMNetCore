using System;
using System.Collections.Generic;
using System.Text;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model.Equipment;

namespace WoMFramework.Game.Model.Actions
{
    public abstract class EntityAction {

        public bool IsExecutable { get; set; }
    }

    public abstract class CombatAction : EntityAction
    {
        public bool ProvokesAttackOfOpportunity;

        public CombatAction(bool provokesAttackOfOpportunity)
        {
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
        }

        public static CombatAction CreateStandardAction(Weapon weapon)
        {
            switch (weapon.WeaponEffortType)
            {
                case WeaponEffortType.Unarmed:
                    return new UnarmedAttack(weapon);
                case WeaponEffortType.Light:
                    return new MeleeAttack(weapon);
                case WeaponEffortType.OneHanded:
                    return new MeleeAttack(weapon);
                case WeaponEffortType.TwoHanded:
                    return new MeleeAttack(weapon);
                case WeaponEffortType.Ranged:
                    return new RangedAttack(weapon);
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

        public StandardAction(Entity target, bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
            Target = target;
        }
    }

    public abstract class WeaponAttack : StandardAction
    {
        public Weapon Weapon { get; }

        public WeaponAttack(Weapon weapon, Entity target, bool provokesAttackOfOpportunity) : base(target, provokesAttackOfOpportunity)
        {
            Weapon = weapon;
        }
    }

    public class UnarmedAttack : WeaponAttack
    {
        public UnarmedAttack(Weapon weapon) : base(weapon, null, true)
        {
            IsExecutable = false;
        }
        private UnarmedAttack(Weapon weapon, Entity target) : base(weapon, target, true)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new UnarmedAttack(Weapon, target);
            return true;
        }
    }

    public class MeleeAttack : WeaponAttack
    {
        public MeleeAttack(Weapon weapon) : base(weapon, null, false)
        {
            IsExecutable = false;
        }
        private MeleeAttack(Weapon weapon, Entity target) : base(weapon, target, false)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new MeleeAttack(Weapon, target);
            return true;
        }
    }

    public class RangedAttack : WeaponAttack
    {
        public RangedAttack(Weapon weapon) : base(weapon, null, true)
        {
            IsExecutable = false;
        }
        private RangedAttack(Weapon weapon, Entity target) : base(weapon, target, true)
        {
            IsExecutable = true;
        }
        public override bool CanExecute(Entity attacker, Entity target, out CombatAction combatAction)
        {
            // TODO: do all necessary checks here !!!
            combatAction = new RangedAttack(Weapon, target);
            return true;
        }
    }

    public class SpellCast : StandardAction
    {
        public SpellCast() : base(null, false)
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
        public MoveAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
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
        public FullRoundAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
        }
    }

    public class FullAttack : FullRoundAction
    {
        public FullAttack() : base(false)
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
        public SwiftAction() : base(false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;        }
    }

    public class ImmediateAction : CombatAction
    {
        public ImmediateAction() : base(false)
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
        public FreeAction() : base(false)
        {
        }

        public override bool CanExecute(Entity attacker, Entity target, out CombatAction standard)
        {
            standard = null;
            return false;
        }
    }

}
