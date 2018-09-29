using System;
using System.Collections.Generic;
using System.Text;

namespace WoMFramework.Game.Model.Actions
{
    public class CombatAction
    {
        public bool ProvokesAttackOfOpportunity;

        public CombatAction(bool provokesAttackOfOpportunity)
        {
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
        } 
    }

    public class StandardAction : CombatAction
    {
        public StandardAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
        }
    }

    public class MeleeAttack : StandardAction
    {
        public MeleeAttack() : base(false)
        {
        }
    }

    public class RangedAttack : StandardAction
    {
        public RangedAttack() : base(true)
        {
        }
    }

    public class UnarmedAttack : StandardAction
    {
        public UnarmedAttack() : base(true)
        {
        }
    }

    public class SpellCast : StandardAction
    {
        public SpellCast() : base(true)
        {
        }
    }

    public class MoveAction : CombatAction
    {
        public MoveAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
        }
    }

    public class FullRoundAction : CombatAction
    {
        public FullRoundAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
        }
    }

    public class SwiftAction : CombatAction
    {
        public SwiftAction() : base(false)
        {
        }
    }

    public class ImmediateAction : CombatAction
    {
        public ImmediateAction() : base(false)
        {
        }
    }

    public class FreeAction : CombatAction
    {
        public FreeAction() : base(false)
        {
        }
    }

}
