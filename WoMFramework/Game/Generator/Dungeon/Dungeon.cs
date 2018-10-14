using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
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

        public const int MaxRoundsPerBlock = 100;

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
        private List<Mogwai> _heroes;

        private readonly List<Monster> _monsters;

        private int _currentRound;

        private int _turn;

        public SimpleDungeon(Shift shift) : base(shift)
        {
            _heroes = new List<Mogwai>();
            _monsters = new List<Monster>();
            _currentRound = 0;
            _turn = 0;

            Map = new Map(DungeonRandom, 58, 58, this);
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
        }

        /// <summary>
        /// Position monster and give them a unique dice.
        /// </summary>
        /// <param name="shift"></param>
        private void DeployMonsters(Shift shift)
        {
            // TODO generate monsters from shift here
            _monsters.Add(Monsters.Rat);

            const int mDeriv = 1000;
            for (var i = 0; i < _monsters.Count; i++)
            {
                // initialize monster
                _monsters[i].Initialize(new Dice(shift, mDeriv + i));
                _monsters[i].Adventure = this;

                // TODO: Positioning monsters
                var monsterCoord = Coord.Get(0, 0);

                for (var x = Map.Width - 1; x >= 0; x--)
                {
                    var found = false;
                    for (var y = Map.Height - 1; y >= 0; y--)
                    {
                        if (Map.WalkabilityMap[x, y])
                        {
                            monsterCoord = Coord.Get(x, y);
                            found = true;
                            break;
                        }
                    }
                    if (found)
                        break;
                }

                Map.AddEntity(_monsters[i], monsterCoord);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        private void DeployMogwai(Mogwai mogwai)
        {
            // TODO generate monsters from shift here
            _heroes.Add(mogwai);

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
            Map.AddEntity(mogwai, mogCoord);
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

            // check if we switch to combat mode and calculate initiative
            if (_heroes.Any(p => p.CombatState == CombatState.Initiation))
            {
                foreach (var entity in Map.GetEntities().OfType<Entity>())
                {
                    entity.CurrentInitiative = entity.InitiativeRoll(entity.Dice);
                    entity.CombatState = CombatState.Engaged;
                    switch (entity)
                    {
                        case Mogwai _:
                            entity.EngagedEnemies = Map.GetEntities().OfType<Monster>().Select(p => p as Entity).ToList();
                            break;
                        case Monster _:
                            entity.EngagedEnemies = Map.GetEntities().OfType<Mogwai>().Select(p => p as Entity).ToList();
                            break;
                    }
                }
            }

            // combat mode
            if (_heroes.Any(p => p.CombatState == CombatState.Engaged))
            {
                CombatRound();
                return;
            }

            // exploration mode
            ExplorationAtom();
        }

        private void ExplorationAtom()
        {
            var _explorationOrder = Map.GetEntities().OfType<Entity>().OrderBy(p => p.Inteligence).ThenBy(p => p.SizeType).ToList();

            // get current combatant
            var entity = _explorationOrder[_turn];

            // increase turn to next initiatives turn
            _turn = ++_turn % _explorationOrder.Count;

            // dead entities don't explore
            if (entity.HealthState < 0)
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
            }

            // dequeue all actions
            while (entityActionQueue.TryDequeue(out var combatAction))
            {
                entity.TakeAction(combatAction);
            }
        }

        public override bool Run(Mogwai mogwai)
        {
            //foreach (var entity in Map.GetEntities().OfType<Entity>())
            //{
            //    entity.CombatState = CombatState.Initiation;
            //}

            for (var _currentRound = 0; _currentRound < MaxRoundsPerBlock && AdventureState == AdventureState.Running; _currentRound++)
            {
                if (mogwai.CombatState == CombatState.Initiation)
                {
                    foreach (var entity in Map.GetEntities().OfType<Entity>())
                    {
                        entity.CurrentInitiative = entity.InitiativeRoll(entity.Dice);
                        entity.CombatState = CombatState.Engaged;
                        switch (entity)
                        {
                            case Mogwai _:
                                entity.EngagedEnemies = Map.GetEntities().OfType<Monster>().Select(p => p as Entity).ToList();
                                break;
                            case Monster _:
                                entity.EngagedEnemies = Map.GetEntities().OfType<Mogwai>().Select(p => p as Entity).ToList();
                                break;
                        }
                    }
                }

                if (mogwai.CombatState == CombatState.Engaged)
                {
                    CombatRound();
                }
                else
                {
                    ExplorationRound();
                }

                // set adventurestate
                EvaluateAdventureState();
            }
            return true;
        }

        private void ExplorationRound()
        {
            var mogwais = Map.GetEntities().OfType<Mogwai>().Where(p => p.HealthState > 0);
            foreach (var entity in mogwais)
            {
                var pois = Map.GetCoords<int>(Map.ExplorationMap, i => i > 0).Where(p => Map.WalkabilityMap[p.X, p.Y]).ToList();
                if (pois.Count == 0)
                {
                    continue;
                }
                var combatActionQueue = new Queue<CombatAction>();
                var poi = Map.Nearest(entity.Coordinate, pois);
                TryEnqueueMove(entity, poi, ref combatActionQueue);

                // dequeue all actions
                while (combatActionQueue.TryDequeue(out var combatAction))
                {
                    entity.TakeAction(combatAction);
                }

            }

            var monsters = Map.GetEntities().OfType<Monster>().Where(p => p.HealthState > 0);
            foreach (var entity in monsters)
            {

            }
        }

        private void CombatRound()
        {
            var inititiveOrdredEntities = Map.GetEntities().OfType<Entity>().Where(p => p.CombatState == CombatState.Engaged).OrderBy(p => p.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
            foreach (var entity in inititiveOrdredEntities)
            {
                var combatActionQueue = new Queue<CombatAction>();

                // dead targets can't attack any more
                if (entity.CurrentHitPoints < 0)
                {
                    continue;
                }

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
                if (target?.CurrentHitPoints < 0 && target is Monster killedMonster && entity is Mogwai)
                {
                    var expReward = killedMonster.Experience;
                    entity.AddExp(expReward, killedMonster);
                }

                // no more combat
                if (!entity.EngagedEnemies.Exists(p => p.CurrentHitPoints > -1))
                {
                    entity.CombatState = CombatState.None;
                }
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
            return entity.EngagedEnemies.FirstOrDefault();
        }

        public override void EvaluateAdventureState()
        {
            UpdateAdventureStats();

            if (AdventureStats[Generator.AdventureStats.Monster] >= 1
             || AdventureStats[Generator.AdventureStats.Explore] >= 1)
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
            var monsters = Map.GetEntities().OfType<Monster>().ToList();
            if (monsters.Count > 0)
            {
                var value = monsters.Count(p => p.HealthState == HealthState.Dead) / monsters.Count;
                AdventureStats[Generator.AdventureStats.Monster] = value;
            }
            else
            {
                AdventureStats[Generator.AdventureStats.Monster] = 1;
            }

            AdventureStats[Generator.AdventureStats.Explore] = Map.GetExplorationState();
        }
    }
}