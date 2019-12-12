namespace WoMFramework.Game.Generator.Dungeon
{
    using GoRogue;
    using GoRogue.MapGeneration;
    using GoRogue.MapViews;
    using Model.Mogwai;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Troschuetz.Random;

    public enum Direction
    {
        Right, Down, Left, Up
    }

    public class Map
    {
        public static readonly Coord[] Directions =
        {
            new Coord(1, 0),    // E
            new Coord(1, -1),   // SE
            new Coord(0, -1),   // S
            new Coord(-1, -1),  // SW
            new Coord(-1, 0),   // W
            new Coord(-1, 1),   // NW
            new Coord(0, 1),    // N
            new Coord(1, 1)     // NE
        };

        public Guid Guid = Guid.NewGuid();

        public Adventure Adventure { get; set; }

        public ArrayMap<bool> WalkabilityMap { get; }
        public ArrayMap<int> ExplorationMap { get; }
        public ArrayMap<AdventureEntityContainer> EntityMap { get; }
        public List<AdventureEntity> Entities { get; }
        public ArrayMap<Tile> TileMap { get; }
        public FOV FovMap { get; }
        public int[,] ExpectedFovNum { get; }

        public List<Rectangle> Locations { get; }

        public int Width { get; }
        public int Height { get; }

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
                //QuickGenerators.GenerateRandomRoomsMap(wMap, dungeonRandom, 15, 5, 15, 50);
                QuickGenerators.GenerateCellularAutomataMap(wMap, dungeonRandom);
            }

            WalkabilityMap = wMap;
            ExplorationMap = new ArrayMap<int>(width, height);
            EntityMap = new ArrayMap<AdventureEntityContainer>(width, height);
            TileMap = new ArrayMap<Tile>(width, height);
            Entities = new List<AdventureEntity>();
            var resMap = new ArrayMap<bool>(width, height);

            foreach (var pos in wMap.Positions())
            {
                // build up Entity Map, not necessary and really slow on big maps
                //EntityMap[i,j] = new AdventureEntityContainer();

                if (wMap[pos.X, pos.Y])
                {
                    //ExplorationMap[i, j] = 1;
                    _walkableTiles++;
                    resMap[pos.X, pos.Y] = true;
                    TileMap[pos.X, pos.Y] = new StoneTile(this, new Coord(pos.X, pos.Y));
                }
                else
                {
                    //ExplorationMap[i, j] = -9;
                    resMap[pos.X, pos.Y] = false;
                    TileMap[pos.X, pos.Y] = new StoneWall(this, new Coord(pos.X, pos.Y));
                }
            }

            FovMap = new FOV(resMap);

            Locations = CreateMapLocations(wMap, 9);

            ExpectedFovNum = new int[width, height];
        }

        private List<Rectangle> CreateMapLocations(ArrayMap<bool> wMap, int minLocationSize)
        {
            var rectangles = new List<Rectangle>();

            foreach (var pos in wMap.Positions())
            {
                if (!wMap[pos.X, pos.Y]) continue;

                var width = 2;
                var height = 2;
                var rectangle = new Rectangle(pos.X, pos.Y, width, height);
                Rectangle legitRectangle = rectangle;
                while (rectangle.Positions().All(p => wMap[p.X, p.Y]))
                {
                    legitRectangle = rectangle;
                    rectangle = rectangle.ChangeSize(++width, ++height);
                }

                if (legitRectangle.Positions().All(p => wMap[p.X, p.Y]))
                {
                    rectangles.Add(legitRectangle);
                }
            }

            //for (var i = 0; i < wMap.Width; i++)
            //{
            //    for (var j = 0; j < wMap.Height; j++)
            //    {
            //        if (!wMap[i, j]) continue;

            //        var width = 2;
            //        var height = 2;
            //        var rectangle = new Rectangle(i, j, width, height);
            //        Rectangle legitRectangle = rectangle;
            //        while (rectangle.Positions().All(p => wMap[p.X, p.Y]))
            //        {
            //            legitRectangle = rectangle;
            //            rectangle = rectangle.ChangeSize(++width, ++height);
            //        }

            //        if (legitRectangle.Positions().All(p => wMap[p.X, p.Y]))
            //        {
            //            rectangles.Add(legitRectangle);
            //        }
            //    }
            //}

            var orderedRectangles = rectangles.Where(p => p.Positions().Count() >= minLocationSize).OrderByDescending(p => p.Positions().Count()).ToList();

            for (var i = 0; i < orderedRectangles.Count - 1; i++)
            {
                for (var j = i + 1; j < orderedRectangles.Count; j++)
                {
                    if (Rectangle.GetUnion(orderedRectangles[i], orderedRectangles[j]) == orderedRectangles[i] || !Rectangle.GetIntersection(orderedRectangles[i], orderedRectangles[j]).IsEmpty)
                    {
                        orderedRectangles[j] = Rectangle.EMPTY;
                    }
                }
            }

            return orderedRectangles.Where(p => !p.IsEmpty).Take(20).ToList();
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
        public void AddEntity(AdventureEntity entity, int x, int y)
        {
            // can't add an entity to invalid position
            if (!WalkabilityMap[x, y])
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[x, y] = false;

            entity.Map = this;
            entity.Coordinate = new Coord(x, y);

            if (EntityMap[x, y] == null)
            {
                EntityMap[x, y] = new AdventureEntityContainer();
            }

            EntityMap[x, y].Add(entity);

            if (entity is Combatant combatant)
            {
                // calculate fov
                combatant.FovCoords = CalculateFoV(entity.Coordinate, entity is Mogwai);
            }

            // add entity to list
            Entities.Add(entity);

            Adventure.Enqueue(AdventureLog.EntityCreated(entity));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public void DeadEntity(Combatant entity)
        {
            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();

            // dead bodies are passable
            entity.IsPassable = true;

            WalkabilityMap[entity.Coordinate] = EntityMap[entity.Coordinate].IsPassable;

            Adventure.Enqueue(AdventureLog.Died(entity));
        }

        /// <summary>
        /// Move an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destination"></param>
        public void MoveEntity(Combatant entity, Coord destination)
        {
            if (!WalkabilityMap[destination])
                throw new Exception();

            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();


            EntityMap[entity.Coordinate].Remove(entity);
            WalkabilityMap[entity.Coordinate] = EntityMap[entity.Coordinate].IsPassable;

            // clean up seems not needed
            //if (!EntityMap[entity.Coordinate].Has<AdventureEntity>())
            //{
            //    EntityMap[entity.Coordinate] = null;
            //}

            if (EntityMap[destination] == null)
            {
                EntityMap[destination] = new AdventureEntityContainer();
            }

            EntityMap[destination].Add(entity);
            WalkabilityMap[destination] = EntityMap[destination].IsPassable;

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
            const int fovrange = 5;

            // calculate fov
            FovMap.Calculate(coords.X, coords.Y, fovrange, Radius.CIRCLE);

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
                var xMin = coords.X - fovrange - 1;
                if (xMin < 0) xMin = 0;
                var xMax = coords.X + fovrange + 1;
                if (xMax >= Width) xMax = Width - 1;
                var yMin = coords.Y - fovrange - 1;
                if (yMin < 0) yMin = 0;
                var yMax = coords.Y + fovrange + 1;
                if (yMax >= Height) yMax = Height - 1;
                var tempResMap = new ArrayMap<bool>(xMax - xMin + 1, yMax - yMin + 1);

                for (var x = xMin; x <= xMax; x++)
                    for (var y = yMin; y <= yMax; y++)
                        tempResMap[x - xMin, y - yMin] = ExplorationMap[x, y] >= 0;

                var tempFOV = new FOV(tempResMap);

                // Calculate expected fov gain for each current fov tiles
                foreach (Coord fovCoord in FovMap.CurrentFOV)
                {
                    // Use translated coordinate instead because tempFOV is viewport.
                    Coord translated = fovCoord.Translate(-xMin, -yMin);

                    tempFOV.Calculate(translated, 5, Radius.CIRCLE);

                    var c = 0;
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

        public List<Combatant> EntitiesOnCoords(HashSet<Coord> coords)
        {
            var combatants = new List<Combatant>();
            foreach (Coord coord in coords)
            {
                AdventureEntityContainer entityContainer = EntityMap[coord];
                if (entityContainer != null && entityContainer.Has<Combatant>())
                {
                    combatants.AddRange(entityContainer.Get<Combatant>());
                }
            }

            return combatants;
        }

        /// <summary>
        /// Remove entity from the map
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveEntity(Combatant entity)
        {
            if (EntityMap[entity.Coordinate] == null)
                throw new Exception();

            if (!entity.IsPassable)
                WalkabilityMap[entity.Coordinate] = true;

            EntityMap[entity.Coordinate] = null;

            // remove entity to list
            Entities.Remove(entity);

            Adventure.Enqueue(AdventureLog.EntityRemoved(entity));
        }

        public static Coord GetDirection(Direction direction)
        {
            return Directions[(int)direction];
        }

        public List<Coord> GetCoords<T>(ArrayMap<T> map, Coord coord, Func<T, bool> validate)
        {
            var coords = new List<Coord>();

            foreach (var position in map.Positions())
            {
                if (validate(map[position.X, position.Y]) && (coord.X != position.X || coord.Y != position.Y))
                {
                    coords.Add(new Coord(position.X, position.Y));
                }
            }

            return coords;
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

        public static Coord Nearest(Coord current, List<Coord> coords)
        {
            Coord nearest = Coord.NONE;
            var distance = double.MaxValue;
            foreach (Coord coord in coords)
            {
                var d = Distance.EUCLIDEAN.Calculate(current, coord);
                if (d >= distance) continue;
                distance = d;
                nearest = coord;
            }

            return nearest;
        }

        public static bool TryGetFirstPatternMatch(ArrayMap<bool> map, ArrayMap<bool> pattern, out Coord startCoord)
        {
            startCoord = new Coord();
            foreach (var pos in map.Positions())
            {
                startCoord = pos;

                var match = true;
                foreach (var patPos in pattern.Positions())
                {
                    var xShift = pos.X + patPos.X;
                    var yShift = pos.Y + patPos.Y;
                    if (map.Width <= xShift 
                     || map.Height <= yShift
                     || map[xShift, yShift] != pattern[patPos.X, patPos.Y]) {
                        match = false;
                        break;
                    }               
                }

                if (match)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
