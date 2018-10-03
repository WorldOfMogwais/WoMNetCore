using System;
using System.Collections.Generic;
using System.Text;

namespace WoMFramework.Game.Model.Actions
{
    public abstract class EntityAction {

    }

    public abstract class CombatAction : EntityAction
    {
        public bool ProvokesAttackOfOpportunity;

        public CombatAction(bool provokesAttackOfOpportunity)
        {
            ProvokesAttackOfOpportunity = provokesAttackOfOpportunity;
        } 
    }

    public abstract class StandardAction : CombatAction
    {
        public Entity[] Targets { get; set; }

        public StandardAction(bool provokesAttackOfOpportunity) : base(provokesAttackOfOpportunity)
        {
        }
    }

    public class MeleeAttack : StandardAction
    {

        // TODO: Feats

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
