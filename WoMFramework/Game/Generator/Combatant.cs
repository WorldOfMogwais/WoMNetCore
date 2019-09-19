namespace WoMFramework.Game.Generator
{
    using Enums;
    using GoRogue;
    using Model;
    using Model.Actions;
    using System.Collections.Generic;

    public abstract class Combatant : AdventureEntity
    {
        public Faction Faction { get; set; }

        public int CurrentInitiative { get; set; }

        public CombatState CombatState { get; set; }

        public List<Entity> EngagedEnemies { get; set; }

        public HashSet<Coord> FovCoords { get; set; }

        public List<CombatAction> CombatActions { get; set; }

        public abstract bool CanSee(AdventureEntity combatant);

        public abstract bool IsInReach(AdventureEntity combatant);

        public abstract bool CanAct { get; }

        public abstract bool IsAlive { get; }

        public abstract bool IsDisabled { get; }

        public abstract bool IsInjured { get; }

        public abstract bool IsDying { get; }

        public abstract bool IsDead { get; }

        public abstract void Reset();

        protected Combatant(bool isStatic, bool isPassable, int size) : base(isStatic, isPassable, size, true)
        {
        }
    }
}
