namespace WoMFramework.Game.Generator
{
    using Dungeon;
    using GoRogue;
    using Interaction;
    using Model;
    using Model.Mogwai;
    using Model.Monster;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public enum AdventureState
    {
        Preparation, 
        Running, 
        Failed, 
        Extended, 
        Completed
    }

    public enum AdventureStats
    {
        Explore, 
        Monster, 
        Boss, 
        Treasure, 
        Portal,
        Gold,
        Experience,
        Item
    }

    public abstract class Adventure
    {
        public abstract Map Map { get; set; }

        public Dictionary<int, Entity> Entities { get; } = new Dictionary<int, Entity>();

        public List<int> BossKeys { get; } = new List<int>();

        public List<Entity> EntitiesList => Entities.Values.ToList();

        public List<Monster> MonstersList => Entities.Values.OfType<Monster>().ToList();

        public List<Mogwai> HeroesList => Entities.Values.OfType<Mogwai>().ToList();

        public AdventureState AdventureState { get; set; }

        public Queue<AdventureLog> AdventureLogs { get; set; } = new Queue<AdventureLog>();

        public Dictionary<AdventureStats, double> AdventureStats { get; }

        public bool CanEnter => AdventureState == AdventureState.Preparation
                             || AdventureState == AdventureState.Extended;

        public Reward Reward { get; set; }

        private int _nextId;
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
                [Generator.AdventureStats.Portal] = 0,
                [Generator.AdventureStats.Gold] = 0,
                [Generator.AdventureStats.Experience] = 0,
                [Generator.AdventureStats.Item] = 0
            };
        }

        public abstract void EvaluateAdventureState();

        public abstract void CreateEntities(Mogwai mogwai, Shift shift);

        public abstract void Enter(Mogwai mogwai, Shift shift);

        public abstract void Prepare(Mogwai mogwai, Shift shift);

        public abstract bool HasNextFrame();

        public abstract void NextFrame();

        public void Enqueue(AdventureLog adventureLog)
        {
            AdventureLogs.Enqueue(adventureLog);
        }
    }

    public class AdventureLog : LogEntry
    {
        public Coord SourceCoord { get; }
        public HashSet<Coord> SourceFovCoords { get; }
        public Coord TargetCoord { get; }
        public int Source { get; }
        public int Target { get; }
        public bool Flag { get; }

        public AdventureLog(LogType type, int source, Coord sourceCoord, HashSet<Coord> sourceFovCoords = null, bool flag = true, ActivityLog activityLog = null) 
            : base(type, "", activityLog)
        {
            Type = type;
            Source = source;
            Target = 0;
            SourceCoord = sourceCoord;
            SourceFovCoords = sourceFovCoords;
            TargetCoord = Coord.NONE;
            Flag = flag;
            ActivityLog = activityLog;
        }

        public AdventureLog(LogType type, int source, Coord sourceCoord, HashSet<Coord> sourceFovCoords, int target, Coord targetCoord, bool flag = true, ActivityLog activityLog = null)
                    : base(type, "", activityLog)
        {
            Type = type;
            Source = source;
            Target = target;
            SourceCoord = sourceCoord;
            SourceFovCoords = sourceFovCoords;
            TargetCoord = targetCoord;
            Flag = flag;
            ActivityLog = activityLog;
        }

        public static AdventureLog EntityCreated(AdventureEntity entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate, entity is Combatant combatant ? combatant.FovCoords : null);
        }

        public static AdventureLog EntityRemoved(AdventureEntity entity)
        {
            return new AdventureLog(LogType.Entity, entity.AdventureEntityId, entity.Coordinate, flag: false);
        }

        public static AdventureLog EntityMoved(Combatant entity, Coord destination)
        {
            return new AdventureLog(LogType.Move, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords, 0, destination);
        }

        public static AdventureLog Attacked(Combatant entity, AdventureEntity target)
        {
            return new AdventureLog(LogType.Attack, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords, target.AdventureEntityId, target.Coordinate);
        }

        public static AdventureLog Died(Combatant entity)
        {
            return new AdventureLog(LogType.Died, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords);
        }

        public static AdventureLog Info(Combatant entity, AdventureEntity target, ActivityLog activityLog)
        {
            return new AdventureLog(LogType.Info, entity.AdventureEntityId, entity.Coordinate, entity.FovCoords, activityLog: activityLog);
        }
    }

    public class ActivityLog
    {
        public enum ActivityType
        {
            Cast,
            Attack,
            Heal,
            Damage,
            HealthState,
            Loot,
            Gold,
            Learn,
            Evolve,
            LevelClass,
            Treasure,
            Exp,
            Level
        }

        public enum ActivityState
        {
            None,
            Init,
            Fail,
            Success
        }

        public ActivityType Type { get; private set; }
        public ActivityState State { get; private set; }
        public int[] Numbers { get; private set; }
        public object ActivityObject { get; private set; }
        public static ActivityLog Create(ActivityType type, ActivityState state, int number, object activityObject)
        {
            return new ActivityLog()
            {
                Type = type,
                State = state,
                Numbers = new int[] { number },
                ActivityObject = activityObject
            };
        }

        public static ActivityLog Create(ActivityType type, ActivityState state, object activityObject)
        {
            return new ActivityLog()
            {
                Type = type,
                State = state,
                Numbers = new int[] { },
                ActivityObject = activityObject
            };
        }

        public static ActivityLog Create(ActivityType type, ActivityState state, int[] numbers, object activityObject)
        {
            return new ActivityLog()
            {
                Type = type,
                State = state,
                Numbers = numbers,
                ActivityObject = activityObject
            };
        }
    }

    public enum CombatState
    {
        None, Initiation, Engaged
    }
}
