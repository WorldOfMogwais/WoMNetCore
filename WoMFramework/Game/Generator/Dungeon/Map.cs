using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using Troschuetz.Random;
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
            Coord.Get(1, 0),    // E
            Coord.Get(1, -1),   // SE
            Coord.Get(0, -1),   // S
            Coord.Get(-1, -1),  // SW
            Coord.Get(-1, 0),   // W
            Coord.Get(-1, 1),   // NW
            Coord.Get(0, 1),    // N
            Coord.Get(1, 1)     // NE
        };

        public Guid Guid = Guid.NewGuid();

        public Adventure Adventure { get; set; }

        public ArrayMap<bool> WalkabilityMap { get; }
        public ArrayMap<int> ExplorationMap { get; }
        public ArrayMap<IAdventureEntity> EntityMap { get; }
        public ArrayMap<Tile> TileMap { get; }
        public FOV FovMap { get; }
        public int[,] ExpectedFovNum { get; }

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
                        //ExplorationMap[i, j] = 1;
                        _walkableTiles++;
                        TileMap[i, j] = new StoneTile(this, Coord.Get(i, j));
                    }
                    else
                    {
                        //ExplorationMap[i, j] = -9;
                        resMap[i, j] = 1;
                        TileMap[i, j] = new StoneWall(this, Coord.Get(i, j));
                    }
                }
            }
            FovMap = new FOV(resMap);

            Locations = CreateMapLocations(wMap, 9);

            ExpectedFovNum = new int[width, height];
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
        public void AddEntity(IAdventureEntity entity, int x, int y)
        {
            // can't add an entity to invalid position
            if (!WalkabilityMap[x, y])
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[x, y] = false;

            entity.Map = this;
            entity.Coordinate = Coord.Get(x, y);
            EntityMap[x, y] = entity;
            EntityCount++;

            if (entity is ICombatant combatant)
            {
                // calculate fov
                combatant.FovCoords = CalculateFoV(entity.Coordinate, entity is Mogwai);
            }

            Adventure.Enqueue(AdventureLog.EntityCreated(entity));
        }

        /// <summary>
        /// Move an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destination"></param>
        public void MoveEntity(ICombatant entity, Coord destination)
        {
            if (!WalkabilityMap[destination])
                throw new Exception();

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
            const int FOVRANGE = 5;

            // calculate fov
            FovMap.Calculate(coords.X, coords.Y, FOVRANGE, Radius.CIRCLE);

            if (isExploring)
            {
                // Visited tile = 2
                ExplorationMap[coords] = 2;

                // Observed reachable tiles = 1
                // Observed impassable tiles = -1 
                foreach (Coord fovCoord in FovMap.CurrentFOV)
                {
                    if (ExplorationMap[fovCoord] != 0) continue;

                    ExplorationMap[fovCoord] = WalkabilityMap[fovCoord] ? 1 : -1;

                }

                // Temporary cropped resistance map.
                // Use exploration map instead of actual walkability map
                // to prevent cheating.

                // Only use local part of the map to reduce memory allocation
                var xMin = coords.X - FOVRANGE - 1;
                if (xMin < 0) xMin = 0;
                var xMax = coords.X + FOVRANGE + 1;
                if (xMax >= Width) xMax = Width - 1;
                var yMin = coords.Y - FOVRANGE - 1;
                if (yMin < 0) yMin = 0;
                var yMax = coords.Y + FOVRANGE + 1;
                if (yMax >= Height) yMax = Height - 1;
                var tempResMap = new ArrayMap<double>(xMax - xMin + 1, yMax - yMin + 1);

                for (int x = xMin; x <= xMax; x++)
                for (int y = yMin; y <= yMax; y++)
                    tempResMap[x - xMin, y - yMin] = ExplorationMap[x, y] < 0 ? 1 : 0;
    
                var tempFOV = new FOV(tempResMap);

                // Calculate expected fov gain for each current fov tiles
                foreach (Coord fovCoord in FovMap.CurrentFOV)
                {
                    // Use translated coordinate instead because tempFOV is viewport.
                    Coord translated = fovCoord.Translate(-xMin, -yMin);

                    tempFOV.Calculate(translated, 5, Radius.CIRCLE);

                    int c = 0;
                    foreach (Coord tempFovCoord in tempFOV.CurrentFOV)
                    {
                        // Don't need to consider already observed tiles
                        if (ExplorationMap[tempFovCoord.Translate(xMin, yMin)] != 0) continue;
                        c++;
                    }

                    // Store expected Number of fov gain
                    ExpectedFovNum[fovCoord.X, fovCoord.Y] = c;

                    // Tiles that don't need to visit = 2
                    if (c == 0)
                        ExplorationMap[fovCoord] = 2;
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

        public List<Coord> GetCoords<T>(ArrayMap<T> map, Coord coord, Func<T, bool> validate)
        {
            var corrds = new List<Coord>();
            for (var i = 0; i < map.Width; i++)
            {
                for (var j = 0; j < map.Height; j++)
                {
                    if (validate(map[i, j]) && (coord.X != i || coord.Y != j))
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
            for (var i = 0; i < ExplorationMap.Width; i++)
            {
                for (var j = 0; j < ExplorationMap.Height; j++)
                {
                    if (ExplorationMap[i, j] == 2)
                    {
                        visited++;
                    }
                }
            }
            return (double)visited / _walkableTiles;
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
