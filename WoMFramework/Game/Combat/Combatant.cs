using System;
using System.Collections.Generic;
using GoRogue;
using GoRogue.Pathing;
using WoMFramework.Game.Model;
using Direction = WoMFramework.Game.Model.Direction;

namespace WoMFramework.Game.Combat
{
    public class Combatant
    {
        private static readonly Distance Distance = Distance.MANHATTAN;

        private Coord _coordinate = Coord.Get(0, 0);
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

        //public Tile CurrentTile
        //{
        //    get => _currentTile;
        //    set
        //    {
        //        _currentTile = value;
        //        value.IsOccupied = true;
        //    }
        //}

        public Coord Coordinate
        {
            get => _coordinate;
            set
            { 
                _room.WalkabilityMap[_coordinate] = true;
                _room.WalkabilityMap[value] = false;
                _coordinate = value;
            }
        }

        public Combatant(Entity entity, Room room)
        {
            Entity = entity;

            _room = room;
            Dungeon = room.Parent;
            _coordinate = entity.Coordinate ?? throw new Exception();

            System.Diagnostics.Debug.WriteLine($"{entity.Name} is spawned at {entity.Coordinate}");

            //  TODO: crude implementation of ranges
            MoveRange = (uint) (entity.Speed / 15); 
            AttackRange = (uint) entity.Equipment.PrimaryWeapon.Range;

            RemainingMove = MoveRange;

            IsMonster = entity is Monster;
            IsHero = entity is Mogwai;
        }

        //private void Move()

        public void Replenish()
        {
            RemainingMove = MoveRange;
        }

        /// <summary>
        /// Attempts to move to an orthogonally adjacent coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public bool TryMove(Coord destination)
        {
            if (RemainingMove == 0)
                return false;

            if (Coord.EuclideanDistanceMagnitude(Coordinate, destination) != 1)
                return false;

            if (!_room.WalkabilityMap[destination])
                return false;

            Coordinate = destination;
            RemainingMove--;
            return true;
        }

        /// <summary>
        /// Move toward the target and stop when the target in in attackable range.
        /// </summary>
        public bool MoveAndTryAttack(Combatant target)
        {
            if (target._room != _room)
                throw new Exception();

            var pathFinding = new AStar(_room.WalkabilityMap, Distance.MANHATTAN);

            var path = pathFinding.ShortestPath(Coordinate, target.Coordinate, true);

            if (path == null)
            {
                if (Distance.Calculate(Coordinate, target.Coordinate) > AttackRange)
                    return false;

                Entity.Attack(0, target.Entity);
                return true;
            }

            for (int i = 0; i < MoveRange; i++)
            {
                if (Distance.Calculate(Coordinate, target.Coordinate) <= AttackRange)
                {
                    System.Diagnostics.Debug.WriteLine($"{Entity.Name} in {Entity.Coordinate} attacked {target.Entity.Name} in {target.Coordinate}");
                    Entity.Attack(0, target.Entity);    // why Entity.Attack() has turn parameter?
                    return true;
                }

                if (!TryMove(path.GetStep(i)))
                    throw new Exception();
            }

            return false;
        }
    }
}
