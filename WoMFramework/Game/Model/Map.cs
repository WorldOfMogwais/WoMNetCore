using System;
using System.Collections.Generic;
using System.Text;
using GoRogue;
using GoRogue.MapGeneration.Generators;
using GoRogue.MapViews;
using WoMFramework.Game.Generator;

namespace WoMFramework.Game.Model
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

        public int Width { get; }
        public int Height { get; }

        public int EntityCount { get; private set; }

        public Map(int width, int height, Adventure adventure)
        {
            Width = width;
            Height = height;
            Adventure = adventure;

            var wMap = new ArrayMap<bool>(width, height);
            RectangleMapGenerator.Generate(wMap);
            WalkabilityMap = wMap;

            EntityMap = new ArrayMap<IAdventureEntity>(width, height);

            //TileMap = new ArrayMap<Tile>(width, height);
            
            //for (int i = 0; i < width; i++)
            //    for (int j = 0; j < height; j++)
            //        if (wMap[i, j])
            //            TileMap[i, j] = 
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

            int k = 0;
            for (int i = 0; i < Width; i++)
            for (int j = 0; j < Height; j++)
                if (EntityMap[i, j] != null)
                    result[k++] = EntityMap[i, j];
            return result;
        }

        public static Coord GetDirection(Direction direction)
        {
            return Directions[(int) direction];
        }
    }
}
