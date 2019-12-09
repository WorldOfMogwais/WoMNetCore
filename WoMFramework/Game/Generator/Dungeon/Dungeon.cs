namespace WoMFramework.Game.Generator.Dungeon
{
    using Enums;
    using GoRogue;
    using Interaction;
    using Model;
    using Model.Actions;
    using Model.Learnable;
    using Model.Mogwai;
    using Model.Monster;
    using Random;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Troschuetz.Random;

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

            var adjCr = GetChallengeRating();

            //var monsterSet = Monsters.Instance.AllBuilders()
            //    .Where(p => (p.EnvironmentTypes.Contains(EnvironmentType.Any)
            //              || p.EnvironmentTypes.Contains(EnvironmentType.Undergrounds))
            //                && p.ChallengeRating <= adjCr).ToList();

            // TODO work here again and replace it with a real algorithm or make it dungeon dependend.
            var monsterSet = new List<MonsterBuilder>() {
                IceCave.BunnyRat,
                IceCave.BearWarrior,
                IceCave.CrystalGuardian,
                IceCave.GoblinFrost,
                IceCave.GoblinMage,
                IceCave.GoblinTorch,
                IceCave.GoblinVenom,
                IceCave.ThreeTailedWolf,
                IceCave.SnowMonster
            };
            
            var totXpAmount = 500 * Math.Pow(adjCr, 2);

            var allMonsters = new List<MonsterBuilder>();

            for (var i = 0; i < 100 && totXpAmount > 0; i++)
            {
                MonsterBuilder mob = monsterSet[DungeonRandom.Next(monsterSet.Count)];
                allMonsters.Add(mob);
                totXpAmount -= mob.Experience;
            }

            // make sure there are at least 7 mobs in the dungeon
            if (Entities.Count < 7)
            {
                var subMonsterSet = monsterSet.Where(p => p.ChallengeRating <= 0.5).ToList();
                if (subMonsterSet.Count > 0)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        MonsterBuilder mob = subMonsterSet[DungeonRandom.Next(subMonsterSet.Count)];
                        allMonsters.Add(mob);
                    }
                }
            }

            var maxCr = allMonsters.Max(p => p.ChallengeRating);
            var potentialBosses = allMonsters.Where(p => p.ChallengeRating == maxCr).ToList();

            Monster boss = potentialBosses[DungeonRandom.Next(potentialBosses.Count)].Build();
            boss.AdventureEntityId = NextId;
            boss.Initialize(new Dice(shift, 1));
            Entities.Add(boss.AdventureEntityId, boss);

            var monsterMod = 100;
            foreach (MonsterBuilder monsterBuilder in allMonsters)
            {
                Monster mob = monsterBuilder.Build();
                mob.AdventureEntityId = NextId;
                mob.Initialize(new Dice(shift, monsterMod++));
                Entities.Add(mob.AdventureEntityId, mob);
            }

            // exploration order
            _explorationOrder = EntitiesList.OrderBy(p => p.Intelligence).ThenBy(p => p.SizeType).ToList();
        }

        private double GetChallengeRating()
        {
            if (ChallengeRating == 0)
            {
                return 0.5;
            }

            if (ChallengeRating == 1)
            {
                return 0.75;
            }

            return ChallengeRating - 1;
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
            Rectangle bossRoom = Map.Locations[0];

            Coord coord = bossRoom.RandomPosition(DungeonRandom);
            while (Map.EntityMap[coord] != null)
            {
                coord = bossRoom.RandomPosition(DungeonRandom);
            }

            var bossChest = new Chest()
            {
                AdventureEntityId = NextId,
                Adventure = this,
                Treasure = new Treasure(1)
            };
            Map.AddEntity(bossChest, coord.X, coord.Y);
        }

        /// <summary>
        /// Position monster and give them a unique dice.
        /// </summary>
        /// <param name="shift"></param>
        private void DeployMonsters(Shift shift)
        {
            Rectangle bossRoom = Map.Locations[0];

            // deploy bosses
            Monster boss = MonstersList.OrderByDescending(p => p.ChallengeRating).First();

            Coord coord = bossRoom.RandomPosition(DungeonRandom);
            while (Map.EntityMap[coord] != null)
            {
                coord = bossRoom.RandomPosition(DungeonRandom);
            }

            boss.Adventure = this;
            Map.AddEntity(boss, coord.X, coord.Y);

            // deploy mobs
            foreach (Monster monster in MonstersList.Where(p => p != boss))
            {
                coord = Coord.NONE;
                while (coord == Coord.NONE || Map.EntityMap[coord] != null)
                {
                    Rectangle location = Map.Locations[DungeonRandom.Next(Map.Locations.Count)];
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
            var mogCoord = new Coord(0, 0);
            for (var x = 0; x < Map.Width; x++)
            {
                var found = false;
                for (var y = 0; y < Map.Height; y++)
                {
                    if (Map.WalkabilityMap[x, y])
                    {
                        mogCoord = new Coord(x, y);
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
                foreach (Entity entity in initiationEntities)
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
            Entity entity = _initiativeOrder[_initiativeTurn];

            // increase turn to next initiatives turn
            _initiativeTurn = ++_initiativeTurn % _initiativeOrder.Count;

            // dead or not engaged entities don't fight
            if (!entity.CanAct || entity.CombatState != CombatState.Engaged)
            {
                return;
            }

            var combatActionQueue = new ConcurrentQueue<CombatAction>();

            // survival check
            if ((double)entity.CurrentHitPoints / entity.MaxHitPoints < 0.25)
            {
                TryEnqueueSurvival(entity, ref combatActionQueue);
            }

            // get a target
            Entity target = GetNearestOrWeakestEnemy(entity);

            // try to attack target
            TryEnqueueAttack(entity, target, ref combatActionQueue, ActionType.Full);

            // need to move to target
            if (combatActionQueue.Count == 0)
            {
                List<Coord> intersects = GetIntersections(entity, target);
                Coord tmp = Map.Nearest(entity.Coordinate, intersects);
                Coord nearestCoord = tmp == Coord.NONE ? target.Coordinate : tmp;

                // enqueue move
                TryEnqueueMove(entity, nearestCoord, ref combatActionQueue);

                if (intersects.Any())
                {
                    // try to attack target
                    TryEnqueueAttack(entity, target, ref combatActionQueue, ActionType.Standard);
                }
            }

            // dequeue all actions
            while (combatActionQueue.TryDequeue(out CombatAction combatAction))
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
            Entity entity = _explorationOrder[_explorationTurn];

            // increase turn to next exploration turn
            _explorationTurn = ++_explorationTurn % _explorationOrder.Count;

            // dead or engaged entities don't explore
            if (!entity.CanAct || entity.CombatState != CombatState.None)
            {
                return;
            }

            // monster exploration might be added later
            if (!(entity is Mogwai mog))
            {
                return;
            }

            // if we have done nothing else we start going on with exploring dungeon ...
            mog.ExploreDungeon(true);
        }

        private void TryEnqueueSurvival(Entity entity, ref ConcurrentQueue<CombatAction> combatActionQueue)
        {
            IEnumerable<CombatAction> healingSpells = entity.CombatActions
                .Where(p => p is SpellCast spellCast && spellCast.Spell.SubSchoolType == SubSchoolType.Healing)
                .Select(p => p.Executable(entity))
                .Where(p => p != null);

            CombatAction healingSpell = healingSpells.FirstOrDefault();

            if (healingSpell != null)
            {
                combatActionQueue.Enqueue(healingSpell);
            }
        }

        private void TryEnqueueMove(Entity entity, Coord nearestCoord, ref ConcurrentQueue<CombatAction> combatActionQueue)
        {
            CombatAction moveAction = entity.CombatActions.Select(p => p.Executable(Map.TileMap[nearestCoord.X, nearestCoord.Y])).FirstOrDefault(p => p is MoveAction);
            if (moveAction != null)
            {
                combatActionQueue.Enqueue(moveAction);
            }
        }

        private void TryEnqueueAttack(Entity entity, Entity target, ref ConcurrentQueue<CombatAction> combatActionQueue, ActionType actionType)
        {
            IEnumerable<CombatAction> exCombatActions = entity.CombatActions
                .Where(p => p.ActionType <= actionType) // only actionTypes that are allowed in this round
                .Where(p => !(p is SpellCast spellCast) || spellCast.Spell.SubSchoolType == SubSchoolType.Injuring)
                .Select(p => p.Executable(target))
                .Where(p => p != null);

            CombatAction combatActionExec = exCombatActions
                .OrderByDescending(p => p.ActionType) // choose full attacks over standard attacks
                .FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack || p is SpellCast);

            if (combatActionExec != null)
            {
                combatActionQueue.Enqueue(combatActionExec);
            }
        }

        private List<Coord> GetIntersections(Entity entity, AdventureEntity target)
        {
            var moveRange = entity.Speed / 5;
            var intersects = new List<Coord>();
            foreach (WeaponAttack weaponAttack in entity.CombatActions.OfType<WeaponAttack>())
            {
                Coord[] attackRadius = new RadiusAreaProvider(target.Coordinate, weaponAttack.GetRange(), Radius.CIRCLE).CalculatePositions().ToArray();
                Coord[] moveRadius = new RadiusAreaProvider(entity.Coordinate, moveRange, Radius.CIRCLE).CalculatePositions().ToArray();
                GoRogue.MapViews.ArrayMap<bool> map = Map.WalkabilityMap;
                intersects.AddRange(from t in attackRadius from coord in moveRadius where coord.X >= 0 && coord.X < map.Width && coord.Y >= 0 && coord.Y < map.Height && map[coord] && coord == t select coord);
            }

            return intersects;
        }

        private Entity GetNearestOrWeakestEnemy(Combatant entity)
        {
            // we hit monsters till they are dead
            IOrderedEnumerable<Entity> orderedEnemiesList = entity.EngagedEnemies.Where(p => p.HealthState != HealthState.Dead)
                .OrderBy(p => Distance.EUCLIDEAN.Calculate(entity.Coordinate, p.Coordinate));

            return orderedEnemiesList.FirstOrDefault();
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
