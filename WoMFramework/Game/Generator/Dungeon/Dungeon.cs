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

        public const int MaxRoundsPerBlock = 250;

        public int CurrentBlockRound;

        public IGenerator DungeonRandom { get; }

        public int ChallengeRating { get; }

        protected Dungeon(Shift shift, int challengeRating)
        {
            Shift = shift;
            ChallengeRating = challengeRating - 1; // challenge rating correction
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
                CurrentBlockRound = 0;
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

        public SimpleDungeon(Shift shift, int challengeRating) : base(shift, challengeRating)
        {
            CurrentBlockRound = 0;
            _currentRound = 0;
            _explorationTurn = 0;
            _initiativeTurn = 0;
            _roundMode = Mode.Exploration;

            Map = new Map(DungeonRandom, 100, 100, this);

            Reward = new Reward(500, 100, null);
        }

        public override void CreateEntities(Mogwai mogwai, Shift shift)
        {
            mogwai.Reset();
            mogwai.AdventureEntityId = NextId;
            Entities.Add(mogwai.AdventureEntityId, mogwai);

            var adjCr = ChallengeRating == 0 ? 0.5 : ChallengeRating;

            var monsterSet = Monsters.Instance.AllBuilders()
                .Where(p => (p.EnvironmentTypes.Contains(EnvironmentType.Any)
                          || p.EnvironmentTypes.Contains(EnvironmentType.Undergrounds))
                            && p.ChallengeRating <= adjCr).ToList();
            var totXpAmount = 500 * Math.Pow(adjCr, 2);

            var allMonsters = new List<MonsterBuilder>();

            for (int i = 0; i < 100 && totXpAmount > 0; i++)
            {
                var mob = monsterSet[DungeonRandom.Next(monsterSet.Count)];
                allMonsters.Add(mob);
                totXpAmount -= mob.Experience;
            }

            // make sure there are at least 7 mobs in the dungeon
            if (Entities.Count < 7)
            {
                var subMonsterSet = monsterSet.Where(p => p.ChallengeRating <= 0.5).ToList();
                for (var i = 0; i < 10; i++)
                {
                    var mob = monsterSet[DungeonRandom.Next(monsterSet.Count)];
                    allMonsters.Add(mob);
                }
            }

            var maxCr = allMonsters.Max(p => p.ChallengeRating);
            var potentialBosses = allMonsters.Where(p => p.ChallengeRating == maxCr).ToList();

            var boss = potentialBosses[DungeonRandom.Next(potentialBosses.Count)].Build();
            boss.AdventureEntityId = NextId;
            boss.Initialize(new Dice(shift, 1));
            Entities.Add(boss.AdventureEntityId, boss);

            int monsterMod = 100;
            foreach (var monsterBuilder in allMonsters)
            {
                var mob = monsterBuilder.Build();
                mob.AdventureEntityId = NextId;
                mob.Initialize(new Dice(shift, monsterMod++));
                Entities.Add(mob.AdventureEntityId, mob);
            }

            // exploration order
            _explorationOrder = EntitiesList.OrderBy(p => p.Inteligence).ThenBy(p => p.SizeType).ToList();
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
            DeployTreasures(shift);
            // deploy traps
            // deploy npc
            // deploy quests


            // deploy mogwai
            DeployMogwai(mogwai);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shift"></param>
        private void DeployTreasures(Shift shift)
        {
            var bossRoom = Map.Locations[0];

            var coord = bossRoom.RandomPosition(DungeonRandom);
            while (Map.EntityMap[coord] != null)
            {
                coord = bossRoom.RandomPosition(DungeonRandom);
            }

            var bossChest = new Chest()
            {
                AdventureEntityId = NextId,
                Adventure = this
            };
            Map.AddEntity(bossChest, coord.X, coord.Y);
        }

        /// <summary>
        /// Position monster and give them a unique dice.
        /// </summary>
        /// <param name="shift"></param>
        private void DeployMonsters(Shift shift)
        {
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
                    entity.CurrentInitiative = entity.InitiativeRoll;
                    entity.CombatState = CombatState.Engaged;
                    entity.EngagedEnemies = initiationEntities.Where(p => p.Faction != entity.Faction).ToList();
                }
                _initiativeTurn = 0;
                _initiativeOrder = initiationEntities.OrderBy(p => p.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
            }

            if (_initiativeTurn == 0 && _roundMode == Mode.Combat || _explorationTurn == 0 && _roundMode == Mode.Exploration)
            {
                CurrentBlockRound++;
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

            // survival check
            if ((double)entity.CurrentHitPoints / entity.MaxHitPoints < 0.25)
            {
                TryEnqueueSurvival(entity, ref combatActionQueue);
            }

            // get a target
            var target = GetNearestOrWeakestEnemy(entity);

            // try to attack target
            TryEnqueueAttack(entity, target, ref combatActionQueue, ActionType.Full);

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
                    TryEnqueueAttack(entity, target, ref combatActionQueue, ActionType.Standard);
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
                if (entity.EngagedEnemies.Contains(target))
                {
                    target.CombatState = CombatState.None;
                    entity.EngagedEnemies.Remove(target);
                }
            }

            // no more combat
            if (entity.EngagedEnemies.Count == 0 || entity.EngagedEnemies.All(p => p.IsDead))
            {
                entity.EngagedEnemies.Clear();
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
                case Mogwai mog:
                    mog.ExploreDungeon(true);
                    break;

                case Monster _:

                    break;
            }
        }

        private void TryEnqueueSurvival(Entity entity, ref Queue<CombatAction> combatActionQueue)
        {
            var healingSpells = entity.CombatActions
                .Where(p => p is SpellCast spellCast && spellCast.Spell.SubSchoolType == SubSchoolType.Healing)
                .Select(p => p.Executable(entity))
                .Where(p => p != null);

            var healingSpell = healingSpells.FirstOrDefault();

            if (healingSpell != null)
            {
                combatActionQueue.Enqueue(healingSpell);
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

        private void TryEnqueueAttack(Entity entity, Entity target, ref Queue<CombatAction> combatActionQueue, ActionType actionType)
        {
            var exCombatActions = entity.CombatActions
                .Where(p => p.ActionType <= actionType) // only actionTypes that are allowed in this round
                .Select(p => p.Executable(target))
                .Where(p => p != null);

            var combatActionExec = exCombatActions
                .OrderByDescending(p => p.ActionType) // choose full attacks over standard attacks
                .FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack);

            if (combatActionExec != null)
            {
                combatActionQueue.Enqueue(combatActionExec);
            }
        }

        private List<Coord> GetIntersections(Entity entity, AdventureEntity target)
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

        private Entity GetNearestOrWeakestEnemy(Combatant entity)
        {
            // we hit monsters till they are dead
            var ordredEnemiesList = entity.EngagedEnemies.Where(p => p.HealthState != HealthState.Dead)
                .OrderBy(p => Distance.EUCLIDEAN.Calculate(entity.Coordinate, p.Coordinate));

            return ordredEnemiesList.FirstOrDefault();
        }

        public override void EvaluateAdventureState()
        {
            UpdateAdventureStats();

            if (HeroesList.All(p => p.IsDead))
            {
                AdventureState = AdventureState.Failed;
            }
            else if (AdventureStats[Generator.AdventureStats.Explore] >= 1)
            {
                AdventureState = AdventureState.Completed;
            }
            else if (CurrentBlockRound >= MaxRoundsPerBlock && _roundMode != Mode.Combat)
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