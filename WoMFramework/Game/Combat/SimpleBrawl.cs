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

    public class SimpleBrawl
    {
        private readonly int _maxRounds;

        private List<Brawler> _inititiveOrder;

        private readonly List<Monster> _monsters;

        private int _currentRound;

        public List<Entity> Heroes => _inititiveOrder.Where(p => p.IsHero).Select(p => p.Entity).ToList();

        public List<Entity> Monsters => _inititiveOrder.Where(p => !p.IsHero).Select(p => p.Entity).ToList();

        public SimpleBrawl(List<Monster> monsters, int maxRounds = 50)
        {
            _monsters = monsters;
            _maxRounds = maxRounds;

            _inititiveOrder = new List<Brawler>();
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
                _inititiveOrder.Add(
                    new Brawler(monster)
                    {
                        InititativeValue = monster.InitiativeRoll(dice),
                        Enemies = new List<Entity> { mogwai }
                    });
            }

            _inititiveOrder.Add(
                new Brawler(mogwai)
                {
                    IsHero = true,
                    InititativeValue = mogwai.InitiativeRoll(mogwai.Dice),
                    Enemies = _monsters.Select(p => p as Entity).ToList()
                });

            _inititiveOrder = _inititiveOrder.OrderBy(s => s.InititativeValue).ThenBy(s => s.Entity.Dexterity).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Run()
        {
            var heros = string.Join(",", _inititiveOrder.Where(p => p.IsHero).Select(p => $"{p.Entity.Name} [{p.InititativeValue}]").ToArray());
            var monsters = string.Join(",", _inititiveOrder.Where(p => !p.IsHero).Select(p => $"{p.Entity.Name} [{p.InititativeValue}]").ToArray());
            Mogwai.History.Add(LogType.Evnt, $"[c:r f:yellow]SimpleBrawl[c:u] {heros} vs. {monsters}");

            Brawler winner = null;

            // let's start the rounds ...
            for (_currentRound = 1; _currentRound < _maxRounds && winner == null; _currentRound++)
            {
                var sec = (_currentRound - 1) * 6;
                Mogwai.History.Add(LogType.Evnt, $"Round {_currentRound:00}, time: {(sec / 60):00}m:{(sec % 60):00}s Monsters: {Monsters.Count} ({string.Join(",", Monsters.Select(p => p.Name))})");

                for (var turn = 0; turn < _inititiveOrder.Count; turn++)
                {
                    var combatant = _inititiveOrder[turn];

                    // dead targets can't attack any more
                    if (combatant.Entity.CurrentHitPoints < 0)
                    {
                        continue;
                    }

                    var target = combatant.Enemies.FirstOrDefault(p => p.CurrentHitPoints > -1);

                    // get all executable combat actions on that target
                    var exCombatActions = new List<CombatAction>();
                    foreach (var combatAction in combatant.Entity.CombatActions)
                    {
                        if (combatAction.CanExecute(combatant.Entity, target, out var exCombatAction))
                        {
                            exCombatActions.Add(exCombatAction);
                        }
                    }

                    // choose just one action here no AI ....
                    var combatActionExec = exCombatActions.FirstOrDefault(p => p is UnarmedAttack || p is MeleeAttack || p is RangedAttack);
                    if (combatActionExec == null)
                    {
                        continue;
                    }

                    // attack
                    combatant.Entity.TakeAction(combatActionExec);

                    // attack
                    //combatant.Entity.Attack(turn, target);

                    if (target?.CurrentHitPoints < 0 && target is Monster killedMonster)
                    {
                        var expReward = killedMonster.Experience / Heroes.Count;
                        Heroes.ForEach(p => p.AddExp(expReward, killedMonster));
                    }

                    if (!combatant.Enemies.Exists(p => p.CurrentHitPoints > -1))
                    {
                        winner = combatant;
                        break;
                    }
                }
            }

            if (winner != null)
            {
                Mogwai.History.Add(LogType.Evnt, $"[c:r f:yellow]SimpleBrawl[c:u] Fight is over! The winner is {Coloring.Name(winner.Entity.Name)}");

                if (winner.IsHero)
                {
                    Loot(Heroes, Monsters);
                    return true;
                }

                return false;
            }

            Mogwai.History.Add(LogType.Evnt, "[c:r f:yellow]SimpleBrawl[c:u] No winner, no loser, this fight was a draw!");
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

