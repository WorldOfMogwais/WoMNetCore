using System;
using System.Collections.Generic;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Combat
{
    public class Combatant
    {
        private Tile _currentTile;
        private readonly Room _room;
        public readonly Dungeon Dungeon;
        public readonly bool IsMonster;
        public readonly bool IsHero;

        public Entity Entity { get; }
        public int InititativeValue { get; set; }
        public Dice Dice { get; set; }
        public List<Combatant> Enemies { get; set; }

        public uint MoveRange { get; private set; }
        public uint AttackRange { get; private set; }
        public uint RemainingMove { get; set; }

        public Tile CurrentTile
        {
            get => _currentTile;
            set
            {
                _currentTile = value;
                value.IsOccupied = true;
            }
        }

        public Combatant(Entity entity, Room room)
        {
            Entity = entity;

            _room = room;
            Dungeon = room.Parent;
            if (room.TryGetTile(entity.Coordinate, out Tile tile))
                _currentTile = tile;
            else
                throw new Exception();

            System.Diagnostics.Debug.WriteLine($"{entity.Name} is spawned at {entity.Coordinate}");

            //  TODO: crude implementation of ranges
            MoveRange = (uint) (entity.Speed / 15); 
            AttackRange = (uint) entity.Equipment.PrimaryWeapon.Range;

            RemainingMove = MoveRange;

            IsMonster = entity is Monster;
            IsHero = entity is Mogwai;
        }

        public void Replenish()
        {
            RemainingMove = MoveRange;
        }


        private void Move(Tile tile)
        {
            CurrentTile.IsOccupied = false;
            CurrentTile = tile;
            RemainingMove--;
            System.Diagnostics.Debug.WriteLine($"{Entity.Name} moves to {CurrentTile}");
        }

        public bool TryMoveTo(Direction direction)
        {
            if (CurrentTile.Sides[(int)direction]?.IsBlocked ?? false)
                return false;

            if (_room.TryGetTile(CurrentTile.Coordinate + Coordinate.Direction(direction), out Tile destination))
            {
                Move(destination);
                return true;
            }

            return false;
        }

        public void MoveTowardDestination(Tile destination)
        {
            if (destination.Parent != _room)
                throw new Exception();

            var path = CurrentTile.GetShortestPath(destination);
            for (int i = 0; i < MoveRange; i++)
                Move(path[i + 1]);
        }

        public void MoveArbitrarily()
        {
            while (true)
            {
                if (TryMoveTo((Direction)Dungeon.DungeonDice.Roll(4, -1)))
                    return;
            }
        }

        /// <summary>
        /// Move toward the target and stop when the target in in attackable range.
        /// </summary>
        public bool MoveAndTryAttack(Combatant target)
        {
            if (target._room != _room)
                throw new Exception();

            var path = _currentTile.GetShortestPath(target.CurrentTile);
            for (int i = 0; i < MoveRange; i++)
            {
                if (path.Length < AttackRange)
                {
                    System.Diagnostics.Debug.WriteLine($"{this.Entity.Name} in {CurrentTile} attacked {target.Entity.Name} in {target.CurrentTile}");
                    Entity.Attack(0, target.Entity);    // why Entity.Attack() has turn parameter?
                    return true;
                }
                Move(path[i + 1]);
            }

            return false;
        }
    }
}
