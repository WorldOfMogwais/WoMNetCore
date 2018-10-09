using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GoRogue;
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

        protected Dungeon(Shift shift)
        {
            Shift = shift;
        }

        public override void NextStep(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                Prepare(mogwai, shift);
                AdventureState = AdventureState.Running;
            }

            if (AdventureState == AdventureState.Running && !Run(mogwai))
            {
                AdventureState = AdventureState.Failed;
            }
        }

        public abstract void Prepare(Mogwai mogwai, Shift shift);

        public abstract bool Run(Mogwai mogwai);

    }

    /// <summary>
    /// Dungeon with one monster room.
    /// Should be removed later.
    /// </summary>
    public class SimpleDungeon : Dungeon
    {
        protected Monster[] SimpleMonsters;

        public SimpleDungeon(Shift shift) : base(shift)
        {
            Map = new Map(36, 13, this);
            SimpleMonsters = new[] { Monsters.Rat };
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
            const int mDeriv = 1000;
            for (var i = 0; i < SimpleMonsters.Length; i++)
            {
                // initialize monster
                SimpleMonsters[i].Initialize(new Dice(shift, mDeriv + i));

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

                Map.AddEntity(SimpleMonsters[i], monsterCoord);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwai"></param>
        private void DeployMogwai(Mogwai mogwai)
        {
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

        public override bool Run(Mogwai mogwai)
        {
            foreach (var entity in Map.GetEntities().OfType<Entity>())
            {
                entity.CombatState = CombatState.Initiation;
            }

            for (var round = 0; round < MaxRoundsPerBlock && AdventureState == AdventureState.Running; round++)
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
            //throw new System.NotImplementedException();
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

                    var nearestCoord = target.Coordinate;
                    var distance = double.MaxValue;

                    foreach (var i in intersects)
                    {
                        var d = Distance.EUCLIDEAN.Calculate(entity.Coordinate, i);
                        if (d >= distance)
                            continue;
                        distance = d;
                        nearestCoord = i;
                    }

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
            var moveAction = entity.CombatActions.Select(p => p.Executable(Map.TileMap[nearestCoord.X,nearestCoord.Y])).FirstOrDefault(p => p is MoveAction);
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

            if (AdventureStats[Generator.AdventureStats.Monster] >= 1)
            {
                AdventureState = AdventureState.Completed;
            }

        }
    }
}