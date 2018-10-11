using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using Troschuetz.Random;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;

namespace WoMFramework.Game.Generator.Dungeon
{
    public enum Direction
    {
        Right, Down, Left, Up
    }

    public class Map
    {
        public static readonly Coord[] Directions =
        {
            Coord.Get(1, 0),
            Coord.Get(0, -1),
            Coord.Get(-1, 0),
            Coord.Get(0, 1)
        };

        public Adventure Adventure { get; set; }

        public ArrayMap<bool> WalkabilityMap { get; }
        public ArrayMap<int> ExplorationMap { get; }
        public ArrayMap<IAdventureEntity> EntityMap { get; }
        public ArrayMap<Tile> TileMap { get; }
        public FOV FovMap { get; }

        public int Width { get; }
        public int Height { get; }

        public int EntityCount { get; private set; }

        public Map(IGenerator dungeonRandom, int width, int height, Adventure adventure)
        {
            Width = width;
            Height = height;
            Adventure = adventure;

            var wMap = new ArrayMap<bool>(width, height);

            // creating map here
            RandomRoomsGenerator.Generate(wMap, dungeonRandom, 12, 5, 9, 20);
            //CellularAutomataGenerator.Generate(wMap, dungeonRandom);

            WalkabilityMap = wMap;
            ExplorationMap = new ArrayMap<int>(width, height);
            EntityMap = new ArrayMap<IAdventureEntity>(width, height);
            TileMap = new ArrayMap<Tile>(width, height);
            var resMap = new ArrayMap<double>(width, height);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (wMap[i, j])
                    {
                        ExplorationMap[i, j] = 1;
                        resMap[i, j] = 0;
                        TileMap[i, j] = new StoneTile(this, Coord.Get(i, j));
                    }
                    else
                    {
                        ExplorationMap[i, j] = 0;
                        resMap[i, j] = 1;
                        TileMap[i, j] = new StoneWall(this, Coord.Get(i, j));
                    }
                }
            }
            FovMap = new FOV(resMap);
        }

        public Map(int width, int height, Adventure adventure)
        {
            Width = width;
            Height = height;
            Adventure = adventure;

            var wMap = new ArrayMap<bool>(width, height);

            // creating map here
            TestMap(wMap);

            WalkabilityMap = wMap;
            ExplorationMap = new ArrayMap<int>(width, height);
            EntityMap = new ArrayMap<IAdventureEntity>(width, height);
            TileMap = new ArrayMap<Tile>(width, height);
            var resMap = new ArrayMap<double>(width, height);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (wMap[i, j])
                    {
                        ExplorationMap[i, j] = 1;
                        resMap[i, j] = 0;
                        TileMap[i, j] = new StoneTile(this, Coord.Get(i, j));
                    }
                    else
                    {
                        ExplorationMap[i, j] = 0;
                        resMap[i, j] = 1;
                        TileMap[i, j] = new StoneWall(this, Coord.Get(i, j));
                    }
                }
            }
            FovMap = new FOV(resMap);
        }

        private void TestMap(ArrayMap<bool> wMap)
        {
            //int minWidth = 3;
            //for (int x = 1; x < wMap.Width - 1; x++)
            //{
            //    double g = Math.Pow(x - (wMap.Width / 2), 2 );
            //    for (int y = 1; y < wMap.Height - 1; y++)
            //    {
            //        if (g > wMap.Height - 2)
            //        {
            //            wMap[x, y] = true;
            //            continue;
            //        }
            //        var halfWidth = (g < minWidth ? minWidth : g) / 2;
            //        var middle = (double) wMap.Height / 2;

            //        if (y < middle  + halfWidth && y > middle - halfWidth)
            //        {
            //            wMap[x, y] = true;
            //        }
            //    }
            //}

            // Rectangle
            for (var x = 1; x < wMap.Width - 1; x++)
                for (var y = 1; y < wMap.Height - 1; y++)
                    wMap[x, y] = true;
        }

        public void AddEntity(ICombatant entity, int x, int y)
        {
            // can't add an entity to invalid position
            if (!WalkabilityMap[x, y])
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[x, y] = false;

            entity.Map = this;
            entity.Coordinate = Coord.Get(x, y);
            entity.AdventureEntityId = Adventure.NextId;
            EntityMap[x, y] = entity;

            EntityCount++;

            // calculate fov
            entity.FovCoords = CalculateFoV(entity.Coordinate);
            //entity.ExploredCoords = GetCoords<int>(ExplorationMap, i => i < 1).ToHashSet();
            Adventure.AdventureLogs.Enqueue(AdventureLog.EntityCreated(entity));
        }

        public void AddEntity(ICombatant entity, Coord pos)
        {
            AddEntity(entity, pos.X, pos.Y);
        }

        public void MoveEntity(ICombatant entity, Coord destination)
        {
            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();

            if (!entity.IsPassable)
            {
                WalkabilityMap[entity.Coordinate] = true;
                WalkabilityMap[destination] = false;
            }
            EntityMap[entity.Coordinate] = null;
            EntityMap[destination] = entity;

            entity.Coordinate = destination;

            entity.FovCoords = CalculateFoV(entity.Coordinate, entity is Mogwai);

            Adventure.AdventureLogs.Enqueue(AdventureLog.EntityMoved(entity, destination));
        }

        private HashSet<Coord> CalculateFoV(Coord coords, bool isExploring = false)
        {
            if (isExploring)
            {
                // exploration fov 1. part
                FovMap.Calculate(coords.X, coords.Y, 4, Radius.CIRCLE);
                foreach (var coord in FovMap.CurrentFOV)
                {
                    ExplorationMap[coord.X, coord.Y] = WalkabilityMap[coord.X, coord.Y] ? 0 : -1;
                }
            }

            // calculate fov
            FovMap.Calculate(coords.X, coords.Y, 5, Radius.CIRCLE);

            if (isExploring)
            {
                // exploration fov 2. part
                foreach (var coord in FovMap.CurrentFOV)
                {
                    if (!WalkabilityMap[coord.X, coord.Y])
                    {
                        ExplorationMap[coord.X, coord.Y] = -1;
                    }
                    else if (ExplorationMap[coord.X, coord.Y] > 0)
                    {
                        ExplorationMap[coord.X, coord.Y] += 1;
                    }
                }
            }

            return new HashSet<Coord>(FovMap.CurrentFOV);
        }

        public void RemoveEntity(ICombatant entity)
        {
            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[entity.Coordinate] = true;

            EntityMap[entity.Coordinate] = null;

            EntityCount--;

            Adventure.AdventureLogs.Enqueue(AdventureLog.EntityRemoved(entity));
        }

        public IAdventureEntity[] GetEntities()
        {
            var result = new IAdventureEntity[EntityCount];

            var k = 0;
            for (var i = 0; i < Width; i++)
                for (var j = 0; j < Height; j++)
                    if (EntityMap[i, j] != null)
                        result[k++] = EntityMap[i, j];
            return result;
        }

        public static Coord GetDirection(Direction direction)
        {
            return Directions[(int)direction];
        }

        public List<Coord> GetCoords<T>(ArrayMap<T> map, Func<T, bool> validate)
        {
            var corrds = new List<Coord>();
            for (var i = 0; i < map.Width; i++)
            {
                for (var j = 0; j < map.Height; j++)
                {
                    if (validate(map[i,j]))
                    {
                        corrds.Add(Coord.Get(i, j));
                    }
                }
            }
            return corrds;
        }

        public double GetExplorationState()
        {
            int walkable = 0;
            int visited = 0;
            for (var i = 0; i < WalkabilityMap.Width; i++)
            {
                for (var j = 0; j < WalkabilityMap.Height; j++)
                {
                    if (!WalkabilityMap[i, j]) continue;

                    walkable++;
                    if (ExplorationMap[i, j] == 0)
                    {
                        visited++;
                    }
                }
            }
            return (double) visited / walkable;
        }

        public Coord Nearest(Coord current, List<Coord> coords)
        {
            Coord nearest = null;
            var distance = double.MaxValue;
            foreach (var coord in coords)
            {
                var d = Distance.EUCLIDEAN.Calculate(current, coord);
                if (d >= distance) continue;
                distance = d;
                nearest = coord;
            }
            return nearest;
        }
    }

}
