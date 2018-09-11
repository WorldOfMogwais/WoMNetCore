using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;
using WoMFramework.Tool;

namespace WoMFramework.Game.Combat
{
    public class SimpleCombat
    {
        private readonly uint _maxRounds;
        private readonly Room _room;
        private readonly List<Combatant> _enemies;
        private readonly List<Combatant> _allies;

        private int currentRound;

        public SimpleCombat(Room room, IEnumerable<Entity> allies, IEnumerable<Monster> monsters, uint maxRounds = 50)
        {
            _maxRounds = maxRounds;

            int m = 1;
            var dice = new Dice(room.Parent.CreationShift, m++);
            _room = room;
            foreach (var monster in monsters)
            {
                monster.Dice = dice;
            }
            _enemies = monsters.Select(p => new Combatant(p, room)
            {
                InititativeValue = p.InitiativeRoll(dice)
            }).ToList();
            _allies = allies.Select(p => new Combatant(p, room)
            {
                InititativeValue = p.InitiativeRoll(p.Dice)
            }).ToList();
        }

        public bool Run()
        {
            var initiativeOrder = _enemies.Concat(_allies).OrderBy(s => s.InititativeValue)
                .ThenBy(s => s.Entity.Dexterity).ToArray();

            var heroes = _allies.Where(p => p.IsHero).Select(p => p.Entity).ToArray();
            List<Entity> killedMonsters = new List<Entity>(_enemies.Count);

            var heroesName = string.Join(",", initiativeOrder.Where(p => !p.IsMonster).Select(p => $"{p.Entity.Name} [{p.InititativeValue}]").ToArray());
            var monsters = string.Join(",", initiativeOrder.Where(p => p.IsMonster).Select(p => $"{p.Entity.Name} [{p.InititativeValue}]").ToArray());
            Mogwai.History.Add(LogType.EVNT, $"¬YSimpleCombat§ [¬C{heroesName}§] vs. [¬C{monsters}§]¬");

            // let's start the rounds ...
            for (currentRound = 1; currentRound < _maxRounds && _enemies.Count > 0; currentRound++)
            {
                int sec = (currentRound - 1) * 6;
                Mogwai.History.Add(LogType.EVNT, $"[ROUND ¬G{currentRound.ToString("00")}§] time: ¬a{(sec / 60).ToString("00")}§m:¬a{(sec % 60).ToString("00")}§s Monsters: {_enemies.Count} ({string.Join(",", _enemies.Select(p => p.Entity.Name))})¬");

                for (int turn = 0; turn < initiativeOrder.Length; turn++)
                {
                    Combatant combatant = initiativeOrder[turn];

                    // dead targets can't attack any more
                    if (combatant.Entity.CurrentHitPoints < 0)
                    {
                        continue;
                    }

                    Combatant target = combatant.IsMonster ? _allies[0] : _enemies[0];

                    // move and attack if available
                    combatant.MoveAndTryAttack(target);

                    if (target.Entity.CurrentHitPoints < 0 && target.IsMonster)
                    {
                        if (target.IsMonster)
                        {
                            Monster killedMonster = (Monster)target.Entity;
                            killedMonsters.Add(killedMonster);

                            int expReward = killedMonster.Experience / heroes.Length;
                            //Heroes.ForEach(p => p.AddExp(expReward, killedMonster));
                            for (int i = 0; i < heroes.Length; i++)
                                heroes[i].AddExp(expReward, killedMonster);

                            _enemies.Remove(target);
                        }
                        else if (target.IsHero)
                        {
                            _allies.Remove(target);
                        }
                    }

                    if (_enemies.Count == 0)
                    {
                        var nameString = heroes.Length == 1 ? $" is {heroes[0].Name}" : $"s are ¬C{string.Join(",", heroes.Select(p => p.Name))}";
                        Mogwai.History.Add(LogType.EVNT, $"¬YSimpleCombat§ Fight is over! The winner{nameString}§¬");

                        Loot(heroes.ToList(), killedMonsters);
                        return true;
                    }

                    if (_allies.Count == 0)
                    {
                        var enemies = _enemies.Select(p => p.Entity).Concat(killedMonsters).ToArray();
                        var nameString = enemies.Length == 1
                            ? $" is {enemies[0].Name}"
                            : $"s are ¬C{string.Join(",", enemies.Select(p => p.Name))}";
                        Mogwai.History.Add(LogType.EVNT, $"¬YSimpleCombat§ Fight is over! The winner{nameString}§¬");
                        return false;

                    }
                }
            }

            Mogwai.History.Add(LogType.EVNT, $"¬YSimpleCombat§ No winner, no loser, this fight was a draw!");
            return false;
        }

        internal void Loot(List<Entity> mogwais, List<Entity> enemies)
        {
            // award experience for each killed enemy
            enemies.ForEach(p =>
            {
                if (p is Monster)
                {
                    Treasure treasure = ((Monster)p).Treasure;
                    string treasureStr = treasure != null ? "¬Ga Treasure§" : "¬Rno Treasure§";
                    Mogwai.History.Add(LogType.EVNT, $"¬YLooting§ the ¬C{p.Name}§ he has {treasureStr}!¬");
                }
            });
        }

    }
}
