using System.Collections.Generic;
using GoRogue;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;

namespace WoMFramework.Game.Generator
{
    public enum AdventureState
    {
        Preparation, Running, Failed, Completed
    }

    public enum AdventureStats
    {
        Explore, Monster, Boss, Treasure, Portal
    }

    public abstract class Adventure
    {
        private int _nextId;

        public abstract Map Map { get; set; }

        public AdventureState AdventureState { get; set; }

        public Queue<AdventureLog> AdventureLogs { get; set; } = new Queue<AdventureLog>();

        public Dictionary<AdventureStats, double> AdventureStats { get; }

        public bool IsActive => AdventureState == AdventureState.Preparation
                             || AdventureState == AdventureState.Running;

        public int NextId => _nextId++;

        protected Adventure()
        {
            AdventureState = AdventureState.Preparation;
            AdventureStats = new Dictionary<AdventureStats, double>
            {
                [Generator.AdventureStats.Explore] = 0,
                [Generator.AdventureStats.Monster] = 0,
                [Generator.AdventureStats.Boss] = 0,
                [Generator.AdventureStats.Portal] = 0
            };
        }

        public abstract void EvaluateAdventureState();

        public abstract void NextStep(Mogwai mogwai, Shift shift);
    }

    public class AdventureLog
    {
        public enum LogType
        {
            Info,
            Move,
            Attack,
            Entity
        }


        public LogType Type { get; }
        public Coord SourceCoord { get; }
        public Coord TargetCoord { get; }
        public int Source { get; }
        public int Target { get; }
        public bool Flag { get; }

        public AdventureLog(LogType type, int source, Coord sourceCoord,
            int target = 0, Coord targetCoord = null, bool flag = true)
        {
            Type = type;
            Source = source;
            Target = target;
            SourceCoord = sourceCoord;
            TargetCoord = targetCoord;
            Flag = flag;
        }

        public static AdventureLog EntityCreated(IAdventureEntity entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate);
        }

        public static AdventureLog EntityRemoved(IAdventureEntity entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate, flag: false);
        }

        public static AdventureLog EntityMoved(IAdventureEntity entity, Coord destination)
        {
            return new AdventureLog(LogType.Move, entity.AdventureEntityId, entity.Coordinate, 0, destination);
        }

        public static AdventureLog Attacked(IAdventureEntity source, IAdventureEntity target)
        {
            return new AdventureLog(LogType.Attack, source.AdventureEntityId, source.Coordinate,
                target.AdventureEntityId, target.Coordinate);
        }
    }

    public interface IAdventureEntity
    {
        Adventure Adventure { get; set; }

        Map Map { get; set; }

        Coord Coordinate { get; set; }

        bool IsStatic { get; }

        bool IsPassable { get; }

        int AdventureEntityId { get; set; }

        int Size { get; }

        bool TakeAction(EntityAction entityAction);
    }

    public enum CombatState
    {
        None, Initiation, Engaged 
    }

    public interface ICombatant : IAdventureEntity
    {
        int CurrentInitiative { get; set; }

        CombatState CombatState { get; set; }

        List<Entity> EngagedEnemies { get; set; }

        void MoveArbitrary();
    }
}