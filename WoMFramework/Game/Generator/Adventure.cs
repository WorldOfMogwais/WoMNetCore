using System.Collections.Generic;
using System.Linq.Expressions;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;

namespace WoMFramework.Game.Generator
{
    public enum AdventureState
    {
        Preparation, Running, Failed, Extended, Completed
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

        public Queue<LogEntry> LogEntries { get; set; } = new Queue<LogEntry>();

        public Dictionary<AdventureStats, double> AdventureStats { get; }

        public bool IsActive => AdventureState == AdventureState.Preparation
                             || AdventureState == AdventureState.Extended
                             || AdventureState == AdventureState.Running;

        public int NextId => _nextId++;

        public abstract int GetRound { get; }

        protected Adventure()
        {
            AdventureState = AdventureState.Preparation;
            AdventureStats = new Dictionary<AdventureStats, double>
            {
                [Generator.AdventureStats.Explore] = 0,
                [Generator.AdventureStats.Monster] = 0,
                [Generator.AdventureStats.Boss] = 0,
                [Generator.AdventureStats.Treasure] = 0,
                [Generator.AdventureStats.Portal] = 0
            };
        }

        public abstract void EvaluateAdventureState();

        public abstract void Enter(Mogwai mogwai, Shift shift);

        public abstract void Prepare(Mogwai mogwai, Shift shift);

        public abstract bool HasNextFrame();

        public abstract void NextFrame();

        public void Enqueue(AdventureLog entityCreated)
        {
            LogEntries.Enqueue(new LogEntry(LogType.AdventureLog, entityCreated.AdventureLogId.ToString()));
            AdventureLogs.Enqueue(entityCreated);
        }
    }

    public class AdventureLog
    {
        public enum LogType
        {
            Info,
            Move,
            Attack,
            Died,
            Entity
        }

        public static int _index = 0;

        public int AdventureLogId { get; }
        public LogType Type { get; }
        public Coord SourceCoord { get; }
        public HashSet<Coord> SourceFovCoords { get; }
        public Coord TargetCoord { get; }
        public int Source { get; }
        public int Target { get; }
        public bool Flag { get; }

        public AdventureLog(LogType type, int source, Coord sourceCoord, HashSet<Coord> sourceFovCoords = null, int target = 0, Coord targetCoord = null, bool flag = true)
        {
            AdventureLogId = _index++;
            Type = type;
            Source = source;
            Target = target;
            SourceCoord = sourceCoord;
            SourceFovCoords = sourceFovCoords;
            TargetCoord = targetCoord;
            Flag = flag;
        }

        public static AdventureLog EntityCreated(ICombatant entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords);
        }

        public static AdventureLog EntityRemoved(ICombatant entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate, flag: false);
        }

        public static AdventureLog EntityMoved(ICombatant entity, Coord destination)
        {
            return new AdventureLog(LogType.Move, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords, 0, destination);
        }

        public static AdventureLog Attacked(ICombatant entity, IAdventureEntity target)
        {
            return new AdventureLog(LogType.Attack, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords, target.AdventureEntityId, target.Coordinate);
        }

        public static AdventureLog Died(ICombatant entity)
        {
            return new AdventureLog(LogType.Died, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords);
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
        Faction Faction { get; set; }

        int CurrentInitiative { get; set; }

        CombatState CombatState { get; set; }

        List<Entity> EngagedEnemies { get; set; }

        HashSet<Coord> FovCoords { get; set; }

        bool CanSee(IAdventureEntity combatant);

        bool CanAct { get; }

        bool IsAlive { get; }

        bool IsDead { get; }
    }
}