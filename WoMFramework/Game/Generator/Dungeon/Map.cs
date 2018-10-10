using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapViews;
using WoMFramework.Game.Model;

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
        public ArrayMap<IAdventureEntity> EntityMap { get; }
        public ArrayMap<Tile> TileMap { get; }
        public FOV FovMap { get; }

        public int Width { get; }
        public int Height { get; }

        public int EntityCount { get; private set; }

        public Map(int width, int height, Adventure adventure)
        {
            Width = width;
            Height = height;
            Adventure = adventure;

            var wMap = new ArrayMap<bool>(width, height);

            // creating map here
            //RectangleMapGenerator.Generate(wMap);
            //RandomRoomsGenerator.Generate(wMap, 2, 11, 11, 20);
            TestMap(wMap);

            WalkabilityMap = wMap;
            EntityMap = new ArrayMap<IAdventureEntity>(width, height);
            TileMap = new ArrayMap<Tile>(width, height);
            var resMap = new ArrayMap<double>(width, height);
            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    if (wMap[i, j])
                    {
                        resMap[i, j] = 0;
                        TileMap[i, j] = new StoneTile(this, Coord.Get(i, j));
                    }
                    else
                    {
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

        public void AddEntity(IAdventureEntity entity, int x, int y)
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

            if (entity is Entity combatant)
            {
                // calculate fov
                FovMap.Calculate(combatant.Coordinate.X, combatant.Coordinate.Y, 5, Radius.CIRCLE);
                combatant.FovCoords = new HashSet<Coord>(FovMap.CurrentFOV);

            }

            Adventure.AdventureLogs.Enqueue(AdventureLog.EntityCreated(entity));
        }

        public void AddEntity(IAdventureEntity entity, Coord pos)
        {
            AddEntity(entity, pos.X, pos.Y);
        }

        public void MoveEntity(IAdventureEntity entity, Coord destination)
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

            Adventure.AdventureLogs.Enqueue(AdventureLog.EntityMoved(entity, destination));
        }

        public void RemoveEntity(IAdventureEntity entity)
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
    }

}
