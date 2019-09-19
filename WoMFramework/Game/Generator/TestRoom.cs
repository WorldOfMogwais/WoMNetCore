namespace WoMFramework.Game.Generator
{
    using Dungeon;
    using GoRogue;
    using Interaction;
    using Model;
    using Model.Actions;
    using Model.Mogwai;
    using Model.Monster;
    using Random;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TestRoom : Adventure
    {
        private int _maxRounds = 50;

        private List<Entity> _initiativeOrder;

        private Entity _winner;

        private int _currentRound;

        private int _turn;

        public override int GetRound => _currentRound;

        public TestRoom()
        {
            _winner = null;
            _currentRound = 0;
            _turn = 0;
            _initiativeOrder = new List<Entity>();
            Map = new Map(null, 10, 10, this, true);
        }

        public override Map Map { get; set; }

        public override void EvaluateAdventureState()
        {
            AdventureState = _currentRound < _maxRounds && _winner == null
                ? AdventureState.Running
                : AdventureState.Completed;
        }

        public override void CreateEntities(Mogwai mogwai, Shift shift)
        {
            mogwai.AdventureEntityId = NextId;
            Entities.Add(mogwai.AdventureEntityId, mogwai);

            var rat1 = Monsters.Instance.ByName("Rat");
            rat1.AdventureEntityId = NextId;
            rat1.Initialize(new Dice(shift, 1));
            Entities.Add(rat1.AdventureEntityId, rat1);

            var rat2 = Monsters.Instance.ByName("Rat");
            rat2.AdventureEntityId = NextId;
            rat2.Initialize(new Dice(shift, 2));
            Entities.Add(rat2.AdventureEntityId, rat2);
        }

        public override void Enter(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                CreateEntities(mogwai, shift);

                Prepare(mogwai, shift);

                AdventureState = AdventureState.Running;
            }

            EvaluateAdventureState();
        }

        public override void Prepare(Mogwai mogwai, Shift shift)
        {
            var coords = new List<Coord>();
            Map.AddEntity(mogwai, 4, 4);
            mogwai.CurrentInitiative = mogwai.InitiativeRoll;
            mogwai.EngagedEnemies = new List<Entity>();
            mogwai.EngagedEnemies.AddRange(MonstersList);

            coords.AddRange(new RadiusAreaProvider(mogwai.Coordinate, 1, Radius.CIRCLE).CalculatePositions().ToList());
            foreach (var monster in MonstersList)
            {
                monster.CurrentInitiative = monster.InitiativeRoll;
                monster.EngagedEnemies =new List<Entity>();
                monster.EngagedEnemies.AddRange(HeroesList);
                monster.Adventure = mogwai.Adventure;

                var getAPlace = coords[1];
                Map.AddEntity(monster, getAPlace.X, getAPlace.Y);
                coords.Remove(getAPlace);
            }

            _initiativeOrder = EntitiesList.OrderBy(s => s.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
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

            if (_turn == 0)
            {
                _currentRound++;
            }

            // get current combatant
            var combatant = _initiativeOrder[_turn];

            // increase turn to next initiatives turn
            _turn = ++_turn % _initiativeOrder.Count;

            // dead targets can't attack any more
            if (!combatant.CanAct)
            {
                return;
            }

            var target = combatant.EngagedEnemies.FirstOrDefault(p => p.IsAlive);
            var exCombatActions = combatant.CombatActions.Select(p => p.Executable(target)).Where(p => p != null);

            // choose just one action here no AI ....
            var combatActionExec = exCombatActions.FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack);
            if (combatActionExec == null)
            {
                return;
            }

            // attack
            combatant.TakeAction(combatActionExec);

            if (target != null && target.IsDead && target is Monster killedMonster)
            {
                var expReward = killedMonster.Experience / HeroesList.Count;
                HeroesList.ForEach(p => p.AddExp(expReward, killedMonster));
            }

            if (!combatant.EngagedEnemies.Exists(p => p.IsAlive))
            {
                _winner = combatant;
            }
        }
    }
}
