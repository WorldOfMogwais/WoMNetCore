using System.Collections.Generic;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public interface ICombatant : IAdventureEntity
    {
        Faction Faction { get; set; }

        int CurrentInitiative { get; set; }

        CombatState CombatState { get; set; }

        List<Entity> EngagedEnemies { get; set; }

        HashSet<Coord> FovCoords { get; set; }

        bool CanSee(IAdventureEntity combatant);

        bool CanAct { get; }

        bool IsAlive { get; }

        bool IsDisabled { get; }

        bool IsInjured { get; }

        bool IsDying { get; }

        bool IsDead { get; }

        void Reset();
    }
}