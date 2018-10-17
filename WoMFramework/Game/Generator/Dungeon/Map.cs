using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GoRogue;
using GoRogue.MapGeneration;
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

        public List<Rectangle> Locations { get; }

        public int Width { get; }
        public int Height { get; }

        public int EntityCount { get; private set; }

        private readonly int _walkableTiles;

        public Map(IGenerator dungeonRandom, int width, int height, Adventure adventure, bool useTestMap = false)
        {
            Width = width;
            Height = height;
            Adventure = adventure;

            var wMap = new ArrayMap<bool>(width, height);

            // creating map here
            if (useTestMap)
            {
                TestMap(wMap);
            }
            else
            {
                //RandomRoomsGenerator.Generate(wMap, dungeonRandom, 15, 5, 15, 50);
                CellularAutomataGenerator.Generate(wMap, dungeonRandom);
            }

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
                        _walkableTiles++;
                        TileMap[i, j] = new StoneTile(this, Coord.Get(i, j));
                    }
                    else
                    {
                        ExplorationMap[i, j] = -9;
                        resMap[i, j] = 1;
                        TileMap[i, j] = new StoneWall(this, Coord.Get(i, j));
                    }
                }
            }
            FovMap = new FOV(resMap);

            Locations = CreateMapLocations(wMap, 9);
        }

        private List<Rectangle> CreateMapLocations(ArrayMap<bool> wMap, int minLocationSize)
        {
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < wMap.Width; i++)
            {
                for (var j = 0; j < wMap.Height; j++)
                {
                    if (!wMap[i, j]) continue;

                    var width = 2;
                    var height = 2;
                    var rectangle = new Rectangle(i, j, width, height);
                    var legitRectangle = rectangle;
                    while (rectangle.Positions().All(p => wMap[p.X, p.Y]))
                    {
                        legitRectangle = rectangle;
                        rectangle = rectangle.SetWidth(++width).SetHeight(++height);
                    }

                    if (legitRectangle.Positions().All(p => wMap[p.X, p.Y]))
                    {
                        rectangles.Add(legitRectangle);
                    }
                    
                    
                }
            }

            var ordredRectangles = rectangles.Where(p => p.Positions().Count() >= minLocationSize).OrderByDescending(p => p.Positions().Count()).ToList();

            for (var i = 0; i < ordredRectangles.Count - 1; i++)
            {
                for (var j = i + 1; j < ordredRectangles.Count; j++)
                {
                    if (Rectangle.GetUnion(ordredRectangles[i], ordredRectangles[j]) == ordredRectangles[i] || !Rectangle.GetIntersection(ordredRectangles[i], ordredRectangles[j]).IsEmpty)
                    {
                        ordredRectangles[j] = Rectangle.EMPTY;
                    }
                }
            }

            return ordredRectangles.Where(p => !p.IsEmpty).Take(20).ToList();
        }

        private void TestMap(ISettableMapView<bool> wMap)
        {
            // Rectangle
            for (var x = 1; x < wMap.Width - 1; x++)
                for (var y = 1; y < wMap.Height - 1; y++)
                    wMap[x, y] = true;
        }

        /// <summary>
        /// Add entity to the map
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void AddEntity(ICombatant entity, int x, int y)
        {
            // can't add an entity to invalid position
            if (!WalkabilityMap[x, y])
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[x, y] = false;

            entity.Map = this;
            entity.Coordinate = Coord.Get(x, y);
            //entity.AdventureEntityId = Adventure.NextId;
            EntityMap[x, y] = entity;
            EntityCount++;

            // calculate fov
            entity.FovCoords = CalculateFoV(entity.Coordinate);
            //entity.ExploredCoords = GetCoords<int>(ExplorationMap, i => i < 1).ToHashSet();
            Adventure.Enqueue(AdventureLog.EntityCreated(entity));
        }

        /// <summary>
        /// Move an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destination"></param>
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

            Adventure.Enqueue(AdventureLog.EntityMoved(entity, destination));
        }

        /// <summary>
        /// Calculate FoV
        /// </summary>
        /// <param name="coords"></param>
        /// <param name="isExploring"></param>
        /// <returns></returns>
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
                    if (ExplorationMap[coord.X, coord.Y] == 0)
                        continue;

                    if (!WalkabilityMap[coord.X, coord.Y])
                    {
                        ExplorationMap[coord.X, coord.Y] = -1;
                    }
                    else
                    {
                        ExplorationMap[coord.X, coord.Y] += 1;
                    }
                }
            }

            return new HashSet<Coord>(FovMap.CurrentFOV);
        }

        public List<ICombatant> EntitiesOnCoords(HashSet<Coord> coords)
        {
            var combatants = new List<ICombatant>();
            foreach (var coord in coords)
            {
                var entity = EntityMap[coord];
                if (entity != null && entity is ICombatant combatant)
                {
                    combatants.Add(combatant);
                }
            }

            return combatants;
        }

        /// <summary>
        /// Remove entity from the map
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(ICombatant entity)
        {
            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[entity.Coordinate] = true;

            EntityMap[entity.Coordinate] = null;

            EntityCount--;

            Adventure.Enqueue(AdventureLog.EntityRemoved(entity));
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
                    if (validate(map[i, j]))
                    {
                        corrds.Add(Coord.Get(i, j));
                    }
                }
            }
            return corrds;
        }

        public double GetExplorationState()
        {
            var visited = 0;
            var unexplored = 0;
            for (var i = 0; i < ExplorationMap.Width; i++)
            {
                for (var j = 0; j < ExplorationMap.Height; j++)
                {
                    if (ExplorationMap[i, j] > 0)
                    {
                        unexplored++;
                    }
                    else if (ExplorationMap[i, j] == 0)
                    {
                        visited++;
                    }
                }
            }
            return (double) visited / (visited + unexplored);
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
