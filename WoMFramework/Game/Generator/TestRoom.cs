using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Generator
{
    public class TestRoom : Adventure
    {
        private int _maxRounds = 50;

        private List<Entity> _inititiveOrder;

        private readonly List<Monster> _monsters;

        private Entity _winner;

        private int _currentRound;

        private int _turn;

        public List<Entity> Heroes => _inititiveOrder.Where(p => p is Mogwai).ToList();

        public List<Entity> Monsters => _inititiveOrder.Where(p => p is Monster).ToList();

        public override int GetRound => _currentRound;

        public TestRoom()
        {
            _winner = null;
            _monsters = new List<Monster> {Model.Monster.Monsters.Rat, Model.Monster.Monsters.Rat};
            _currentRound = 0;
            _turn = 0;
            _inititiveOrder = new List<Entity>();
            Map = new Map(null, 10, 10, this, true);
        }

        public override Map Map { get; set; }

        public override void EvaluateAdventureState()
        {
            AdventureState = _currentRound < _maxRounds && _winner == null
                ? AdventureState.Running
                : AdventureState.Completed;
        }

        public override void Enter(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                Prepare(mogwai, shift);
                AdventureState = AdventureState.Running;
            }

            EvaluateAdventureState();
        }

        public override void Prepare(Mogwai mogwai, Shift shift)
        {
            var coords = new List<Coord>();
            Map.AddEntity(mogwai, 4, 4);
            mogwai.CurrentInitiative = mogwai.InitiativeRoll(mogwai.Dice);
            mogwai.EngagedEnemies = new List<Entity>();
            mogwai.EngagedEnemies.AddRange(_monsters);
            _inititiveOrder.Add(mogwai);

            coords.AddRange(new RadiusAreaProvider(mogwai.Coordinate, 1, Radius.CIRCLE).CalculatePositions().ToList());
            var m = 1;
            foreach (var monster in _monsters)
            {
                var dice = new Dice(shift, m++);
                monster.Initialize(dice);
                monster.CurrentInitiative = monster.InitiativeRoll(dice);
                monster.EngagedEnemies = new List<Entity> { mogwai };
                monster.Adventure = mogwai.Adventure;
                _inititiveOrder.Add(monster);

                var getAPlace = coords[1];
                Map.AddEntity(monster, getAPlace.X, getAPlace.Y);
                coords.Remove(getAPlace);
            }

            _inititiveOrder = _inititiveOrder.OrderBy(s => s.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
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
            var combatant = _inititiveOrder[_turn];

            // increase turn to next initiatives turn
            _turn = ++_turn % _inititiveOrder.Count;

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
                var expReward = killedMonster.Experience / Heroes.Count;
                Heroes.ForEach(p => p.AddExp(expReward, killedMonster));
            }

            if (!combatant.EngagedEnemies.Exists(p => p.IsAlive))
            {
                _winner = combatant;
            }
        }
    }
}