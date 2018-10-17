using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapGeneration;
using Troschuetz.Random;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Generator.Dungeon
{
    public abstract class Dungeon : Adventure
    {
        protected readonly Shift Shift;

        public override Map Map { get; set; }

        public const int MaxRoundsPerBlock = 250;

        public IGenerator DungeonRandom { get; }

        protected Dungeon(Shift shift)
        {
            Shift = shift;
            DungeonRandom = new Dice(shift).GetRandomGenerator();
        }

        public override void Enter(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                CreateEntities(mogwai, shift);

                Prepare(mogwai, shift);

                AdventureState = AdventureState.Running;
            }

            // TODO implement dungeons to go over multiple blocks
            if (AdventureState == AdventureState.Extended)
            {
                //Prepare(mogwai, shift);
                AdventureState = AdventureState.Running;
            }

            EvaluateAdventureState();
        }

    }

    /// <summary>
    /// Dungeon with one monster room.
    /// Should be removed later.
    /// </summary>
    public class SimpleDungeon : Dungeon
    {
        private enum Mode
        {
            Exploration,
            Combat
        };

        private List<Entity> _explorationOrder;

        private int _explorationTurn;

        private List<Entity> _initiativeOrder;

        private int _initiativeTurn;

        // every 10 combat rounds there is a exploration round
        private int _combatExploFactor = 10;

        private int _currentRound;

        private Mode _roundMode;

        public override int GetRound => _currentRound;

        public SimpleDungeon(Shift shift) : base(shift)
        {
            _currentRound = 0;
            _explorationTurn = 0;
            _initiativeTurn = 0;
            _roundMode = Mode.Exploration;

            Map = new Map(DungeonRandom, 58, 58, this);
        }

        public override void CreateEntities(Mogwai mogwai, Shift shift)
        {
            mogwai.AdventureEntityId = NextId;
            Entities.Add(mogwai.AdventureEntityId, mogwai);

            var boss = Monsters.DireRat;
            boss.AdventureEntityId = NextId;
            boss.Initialize(new Dice(shift, 1));
            Entities.Add(boss.AdventureEntityId, boss);

            for (var i = 0; i < 6; i++)
            {
                var mob = Monsters.Rat;
                mob.AdventureEntityId = NextId;
                mob.Initialize(new Dice(shift, i + 99));
                Entities.Add(mob.AdventureEntityId, mob);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        /// <param name="shift"></param>
        public override void Prepare(Mogwai mogwai, Shift shift)
        {
            // deploy monster, monster packs
            DeployMonsters(shift);

            // deploy boss monsters
            // deploy treasures
            // deploy traps
            // deploy npc
            // deploy quests


            // deploy mogwai
            DeployMogwai(mogwai);

            // TODO add this to constructor once all entities are generated from shift
            _explorationOrder = EntitiesList.OrderBy(p => p.Inteligence).ThenBy(p => p.SizeType).ToList();
            //_explorationOrder = new List<Entity> {mogwai};
        }

        /// <summary>
        /// Position monster and give them a unique dice.
        /// </summary>
        /// <param name="shift"></param>
        private void DeployMonsters(Shift shift)
        {
            // TODO generate monsters from shift here
            var bossRoom = Map.Locations[0];

            // deploy bosses
            var boss = MonstersList.OrderByDescending(p => p.ChallengeRating).First();

            var coord = bossRoom.RandomPosition(DungeonRandom);
            while (Map.EntityMap[coord] != null)
            {
                coord = bossRoom.RandomPosition(DungeonRandom);
            }
            boss.Adventure = this;
            Map.AddEntity(boss, coord.X, coord.Y);

            // deploy mobs
            foreach (var monster in MonstersList.Where(p => p != boss))
            {
                coord = null;
                while (coord == null || Map.EntityMap[coord] != null)
                {
                    var location = Map.Locations[DungeonRandom.Next(Map.Locations.Count)];
                    coord = location.RandomPosition(DungeonRandom);
                }
                monster.Adventure = this;
                Map.AddEntity(monster, coord.X, coord.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        private void DeployMogwai(Mogwai mogwai)
        {
            // TODO generate monsters from shift here
            var mogCoord = Coord.Get(0, 0);
            for (var x = 0; x < Map.Width; x++)
            {
                var found = false;
                for (var y = 0; y < Map.Height; y++)
                {
                    if (Map.WalkabilityMap[x, y])
                    {
                        mogCoord = Coord.Get(x, y);
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            Map.AddEntity(mogwai, mogCoord.X, mogCoord.Y);
        }

        public override bool HasNextFrame()
        {
            EvaluateAdventureState();

            return AdventureState == AdventureState.Running;
        }

        public override void NextFrame()
        {
            if (!HasNextFrame())
            {
                throw new Exception("There is no next frame possible.");
            }

            var initiationEntities = Entities.Values.Where(p => p.CombatState == CombatState.Initiation).ToList();
            
            // check if we switch to combat mode and calculate initiative
            if (initiationEntities.Any())
            {
                foreach (var entity in initiationEntities)
                {
                    entity.CurrentInitiative = entity.InitiativeRoll(entity.Dice);
                    entity.CombatState = CombatState.Engaged;
                    entity.EngagedEnemies = initiationEntities.Where(p => p.Faction != entity.Faction).ToList();
                }
                _initiativeTurn = 0;
                _initiativeOrder = initiationEntities.OrderBy(p => p.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
            }

            if (_initiativeTurn == 0 && _roundMode == Mode.Combat || _explorationTurn == 0 && _roundMode == Mode.Exploration)
            {
                _currentRound++;
            }

            if (_initiativeOrder == null || _initiativeOrder.Count == 0)
            {
                _roundMode = Mode.Exploration;
            }
            else if (_roundMode == Mode.Exploration && _explorationTurn == 0)
            {
                _roundMode = Mode.Combat;
            }
            else if (_initiativeTurn == 0 && _currentRound % _combatExploFactor == 0)
            {
                _roundMode = Mode.Exploration;
            }

            switch (_roundMode)
            {
                case Mode.Exploration:
                    ExplorationAtom();
                    break;

                case Mode.Combat:
                    CombatAtom();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private void CombatAtom()
        {
            // get current combatant
            var entity = _initiativeOrder[_initiativeTurn];

            // increase turn to next initiatives turn
            _initiativeTurn = ++_initiativeTurn % _initiativeOrder.Count;

            // dead or not engaged entities don't fight
            if (!entity.CanAct || entity.CombatState != CombatState.Engaged)
            {
                return;
            }

            var combatActionQueue = new Queue<CombatAction>();

            // get a target
            var target = GetNearestOrWeakestEnemy(entity);

            // try to attack target
            TryEnqueueAttack(entity, target, ref combatActionQueue);

            // need to move to target
            if (combatActionQueue.Count == 0)
            {
                var intersects = GetIntersections(entity, target);
                var nearestCoord = Map.Nearest(entity.Coordinate, intersects) ?? target.Coordinate;

                // enqueue move
                TryEnqueueMove(entity, nearestCoord, ref combatActionQueue);

                if (intersects.Any())
                {
                    // try to attack target
                    TryEnqueueAttack(entity, target, ref combatActionQueue);
                }
            }

            // dequeue all actions
            while (combatActionQueue.TryDequeue(out var combatAction))
            {
                entity.TakeAction(combatAction);
            }

            // reward xp for killed monsters                
            if (target.IsDead && target is Monster killedMonster && entity is Mogwai)
            {
                var expReward = killedMonster.Experience;
                entity.AddExp(expReward, killedMonster);
            }

            // no more combat
            if (entity.EngagedEnemies.All(p => p.IsDead))
            {
                entity.CombatState = CombatState.None;

                // reset combat stuff
                _initiativeOrder.Clear();
                _initiativeTurn = 0;
            }
        }

        private void ExplorationAtom()
        {
            // get current combatant
            var entity = _explorationOrder[_explorationTurn];

            // increase turn to next exploration turn
            _explorationTurn = ++_explorationTurn % _explorationOrder.Count;

            // dead or engaged entities don't explore
            if (!entity.CanAct || entity.CombatState != CombatState.None)
            {
                return;
            }

            var entityActionQueue = new Queue<CombatAction>();

            switch (entity)
            {
                case Mogwai _:
                    var pois = Map.GetCoords(Map.ExplorationMap, i => i > 0).Where(p => Map.WalkabilityMap[p.X, p.Y]).ToList();
                    if (pois.Count == 0)
                    {
                        return;
                    }
                    var poi = Map.Nearest(entity.Coordinate, pois);
                    TryEnqueueMove(entity, poi, ref entityActionQueue);
                    break;

                case Monster _:
                    //Coord destination;
                    //do
                    //{
                    //    var roll = entity.Dice.Roll(4, -1);
                    //    destination = entity.Coordinate + Map.Directions[roll];
                    //} while (!Map.WalkabilityMap[destination]);
                    //TryEnqueueMove(entity, destination, ref entityActionQueue);
                    break;
            }

            // dequeue all actions
            while (entityActionQueue.TryDequeue(out var combatAction))
            {
                entity.TakeAction(combatAction);
            }
        }

        private void TryEnqueueMove(Entity entity, Coord nearestCoord, ref Queue<CombatAction> combatActionQueue)
        {
            var moveAction = entity.CombatActions.Select(p => p.Executable(Map.TileMap[nearestCoord.X, nearestCoord.Y])).FirstOrDefault(p => p is MoveAction);
            if (moveAction != null)
            {
                combatActionQueue.Enqueue(moveAction);
            }
        }

        private void TryEnqueueAttack(Entity entity, Entity target, ref Queue<CombatAction> combatActionQueue)
        {
            var exCombatActions = entity.CombatActions.Select(p => p.Executable(target)).Where(p => p != null);
            var combatActionExec = exCombatActions.FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack);
            if (combatActionExec != null)
            {
                combatActionQueue.Enqueue(combatActionExec);
            }
        }

        private List<Coord> GetIntersections(Entity entity, IAdventureEntity target)
        {
            var moveRange = entity.Speed / 5;
            var intersects = new List<Coord>();
            foreach (var weaponAttack in entity.CombatActions.OfType<WeaponAttack>())
            {
                var attackRadius = new RadiusAreaProvider(target.Coordinate, weaponAttack.GetRange(), Radius.CIRCLE).CalculatePositions().ToArray();
                var moveRadius = new RadiusAreaProvider(entity.Coordinate, moveRange, Radius.CIRCLE).CalculatePositions().ToArray();
                var map = Map.WalkabilityMap;
                intersects.AddRange(from t in attackRadius from coord in moveRadius where coord.X >= 0 && coord.X < map.Width && coord.Y >= 0 && coord.Y < map.Height && map[coord] && coord == t select coord);
            }

            return intersects;
        }

        private Entity GetNearestOrWeakestEnemy(ICombatant entity)
        {
            // we hit monsters till they are dead
            var ordredEnemiesList = entity.EngagedEnemies.Where(p => p.HealthState != HealthState.Dead)
                .OrderBy(p => Distance.EUCLIDEAN.Calculate(entity.Coordinate, p.Coordinate));

            return ordredEnemiesList.FirstOrDefault();
        }

        public override void EvaluateAdventureState()
        {
            UpdateAdventureStats();

            if (AdventureStats[Generator.AdventureStats.Monster] >= 1
             && AdventureStats[Generator.AdventureStats.Explore] >= 1)
            {
                AdventureState = AdventureState.Completed;

            }
            else if (_currentRound >= MaxRoundsPerBlock)
            {
                AdventureState = AdventureState.Extended;
            }
        }

        private void UpdateAdventureStats()
        {
            if (MonstersList.Any())
            {
                AdventureStats[Generator.AdventureStats.Monster] = (double)MonstersList.Count(p => p.IsDead) / MonstersList.Count;
            }
            else
            {
                AdventureStats[Generator.AdventureStats.Monster] = 1;
            }

            AdventureStats[Generator.AdventureStats.Explore] = Map.GetExplorationState();
        }
    }
}