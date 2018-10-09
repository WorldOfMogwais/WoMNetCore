using System.Collections.Generic;
using System.Linq;
using WoMFramework.Game.Enums;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Combat
{
    public class SimpleCombat
    {
        private readonly int _maxRounds;

        private List<Entity> _inititiveOrder;

        private readonly List<Monster> _monsters;

        private int _currentRound;

        public List<Entity> Heroes => _inititiveOrder.Where(p => p is Mogwai).ToList();

        public List<Entity> Monsters => _inititiveOrder.Where(p => p is Monster).ToList();

        public SimpleCombat(List<Monster> monsters, int maxRounds = 50)
        {
            _monsters = monsters;
            _maxRounds = maxRounds;

            _inititiveOrder = new List<Entity>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Prepare(Mogwai mogwai, Shift shift)
        {
            var m = 1;
            foreach (var monster in _monsters)
            {
                var dice = new Dice(shift, m++);
                monster.Initialize(dice);
                monster.CurrentInitiative = monster.InitiativeRoll(dice);
                monster.EngagedEnemies = new List<Entity> {mogwai};
                monster.Adventure = mogwai.Adventure;
                _inititiveOrder.Add(monster);
            }

            mogwai.CurrentInitiative = mogwai.InitiativeRoll(mogwai.Dice);
            mogwai.EngagedEnemies = new List<Entity>();
            mogwai.EngagedEnemies.AddRange(_monsters);
            _inititiveOrder.Add(mogwai);

            _inititiveOrder = _inititiveOrder.OrderBy(s => s.CurrentInitiative).ThenBy(s => s.Dexterity).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            var heros = string.Join(",", Heroes.Select(p => $"{p.Name} [{p.CurrentInitiative}]").ToArray());
            var monsters = string.Join(",", Monsters.Select(p => $"{p.Name} [{p.CurrentInitiative}]").ToArray());
            Mogwai.History.Add(LogType.Evnt, $"[c:r f:yellow]SimpleCombat[c:u] {heros} vs. {monsters}");

            Entity winner = null;

            // let's start the rounds ...
            for (_currentRound = 1; _currentRound < _maxRounds && winner == null; _currentRound++)
            {
                var sec = (_currentRound - 1) * 6;
                Mogwai.History.Add(LogType.Evnt, $"Round {_currentRound:00}, time: {(sec / 60):00}m:{(sec % 60):00}s Monsters: {Monsters.Count} ({string.Join(",", Monsters.Select(p => p.Name))})");

                for (var turn = 0; turn < _inititiveOrder.Count; turn++)
                {
                    var combatant = _inititiveOrder[turn];

                    // dead targets can't attack any more
                    if (combatant.CurrentHitPoints < 0)
                    {
                        continue;
                    }

                    var target = combatant.EngagedEnemies.FirstOrDefault(p => p.CurrentHitPoints > -1);

                    // get all executable combat actions on that target
                    //var exCombatActions = new List<CombatAction>();
                    var exCombatActions = combatant.CombatActions.Select(p => p.Executable(target)).Where(p => p != null);
                    //foreach (var combatAction in combatant.Entity.CombatActions)
                    //{
                    //    var tCombatAction = combatAction.Executable(target);
                    //    if (tCombatAction != null)
                    //    {
                    //        exCombatActions.Add(tCombatAction);
                    //    }
                    //}

                    // choose just one action here no AI ....
                    var combatActionExec = exCombatActions.FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack);
                    if (combatActionExec == null)
                    {
                        continue;
                    }

                    // attack
                    combatant.TakeAction(combatActionExec);

                    // attack
                    //combatant.Entity.Attack(turn, target);

                    if (target?.CurrentHitPoints < 0 && target is Monster killedMonster)
                    {
                        var expReward = killedMonster.Experience / Heroes.Count;
                        Heroes.ForEach(p => p.AddExp(expReward, killedMonster));
                    }

                    if (!combatant.EngagedEnemies.Exists(p => p.CurrentHitPoints > -1))
                    {
                        winner = combatant;
                        break;
                    }
                }
            }

            if (winner != null)
            {
                Mogwai.History.Add(LogType.Evnt, $"[c:r f:yellow]SimpleCombat[c:u] Fight is over! The winner is {Coloring.Name(winner.Name)}");

                if (winner is Mogwai)
                {
                    Loot(Heroes, Monsters);
                    return true;
                }

                return false;
            }

            Mogwai.History.Add(LogType.Evnt, "[c:r f:yellow]SimpleCombat[c:u] No winner, no loser, this fight was a draw!");
            return false;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mogwais"></param>
        /// <param name="enemies"></param>
        internal void Loot(List<Entity> mogwais, List<Entity> enemies)
        {
            // award experience for each killed enemy
            enemies.ForEach(p =>
            {
                if (p is Monster monster)
                {
                    var treasure = monster.TreasureType;
                    var treasureStr = treasure != TreasureType.None ? "[c:r f:gold]a Treasure[c:u]" : "[c:r f:red]no Treasure[c:u]";
                    Mogwai.History.Add(LogType.Evnt, $"[c:r f:darkorange]Looting[c:u] the {Coloring.Name(monster.Name)} he has {treasureStr}!");
                }
            });
        }

    }
}

