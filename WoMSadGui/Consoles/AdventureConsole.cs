using System;
using System.Collections.Generic;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Generator.Dungeon;
using WoMWallet.Node;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using Console = SadConsole.Console;
using ConsoleEntity = SadConsole.Entities.Entity;

namespace WoMSadGui.Consoles
{
    public class AdventureConsole : MogwaiConsole
    {
        private static readonly Cell UnknownAppearance = new Cell(new Color(25,25,25), Color.Black, 219);
        private static readonly Cell UnclearAppearance = new Cell(new Color(50,150,0,100), Color.Black, 219);
        private static readonly Cell StoneTileAppearance = new Cell(Color.DarkGray, Color.Black, 46);
        private static readonly Cell StoneWallAppearance = new Cell(Color.DarkGray, Color.Black, 35);

        private readonly Font AdventureFont;

        private readonly MogwaiController _controller;
        private readonly MogwaiKeys _mogwaiKeys;
        private readonly Mogwai _mogwai;

        private readonly Console _mapConsole;

        private readonly Console _statsConsole;

        private readonly Dictionary<int, ConsoleEntity> _entities = new Dictionary<int, ConsoleEntity>();

        public static TimeSpan GameSpeed = TimeSpan.FromSeconds(0.1);
        public DateTime LastUpdate;

        public Adventure Adventure { get; private set; }

        public AdventureConsole(MogwaiController mogwaiController, MogwaiKeys mogwaiKeys, int width, int height) : base("Custom", "", width, height)
        {
            _controller = mogwaiController;
            _mogwaiKeys = mogwaiKeys;
            _mogwai = _mogwaiKeys.Mogwai;

            AdventureFont = Global.LoadFont("Cheepicus12.font").GetFont(Font.FontSizes.Half);

            _mapConsole = new Console(75, 75) { Position = new Point(17, 2) };
            _mapConsole.Font = AdventureFont;
            Children.Add(_mapConsole);

            _statsConsole = new MogwaiConsole("stats", "", 21, 5) { Position = new Point(70, 17) };
            Children.Add(_statsConsole);

            _mapConsole.IsVisible = false;

            _statsConsole.Print(7, 0, "Exploration", Color.Gainsboro);
            _statsConsole.Print(7, 1, "Monster", Color.Gainsboro);
            _statsConsole.Print(7, 2, "Boss", Color.Gainsboro);
            _statsConsole.Print(7, 3, "Treasure", Color.Gainsboro);
            _statsConsole.Print(7, 4, "Portal", Color.Gainsboro);
        }


        public bool IsStarted()
        {
            return _mapConsole.IsVisible;
        }

        public void Start(Adventure adventure)
        {
            _mapConsole.IsVisible = true;

            _mapConsole.Children.Clear();
            _entities.Clear();

            Adventure = adventure;

            DrawExploMap();
            //DrawMap();

            // Draw entities (Mogwais, Monsters, etc.)
            foreach (var entity in Adventure.Map.GetEntities())
            {
                if (!entity.IsStatic)
                {
                    DrawEntity(entity);
                }
            }

            LastUpdate = DateTime.Now;
        }

        public void Stop()
        {
            _mapConsole.IsVisible = false;

            _mapConsole.Children.Clear();
            _entities.Clear();
        }

        public void DrawMap()
        {
            // TODO: Viewport
            //Resize(Adventure.Map.Width, Adventure.Map.Height, true);

            var wMap = Adventure.Map.TileMap;
            for (var i = 0; i < wMap.Width; i++)
            {
                for (var j = 0; j < wMap.Height; j++)
                {
                    switch (wMap[i, j])
                    {
                        case StoneTile _:
                            _mapConsole[i, j].CopyAppearanceFrom(StoneTileAppearance);
                            _mapConsole.SetGlyph(i, j, 46);
                            break;
                        case StoneWall _:
                            _mapConsole[i, j].CopyAppearanceFrom(StoneWallAppearance);
                            _mapConsole.SetGlyph(i, j, 35);
                            break;
                        default:
                            break;

                    }
                }
            }
        }

        public void DrawExploMap()
        {
            var wMap = Adventure.Map.ExplorationMap;
            for (var i = 0; i < wMap.Width; i++)
            {
                for (var j = 0; j < wMap.Height; j++)
                {
                    switch (wMap[i, j])
                    {
                        case 0:
                        case -1:
                            DrawMapPoint(i, j);
                            break;
                        case -2:
                        case 1:
                            _mapConsole[i, j].CopyAppearanceFrom(UnknownAppearance);
                            _mapConsole.SetGlyph(i, j, 219);
                            break;
                        default:
                            _mapConsole[i, j].CopyAppearanceFrom(UnclearAppearance);
                            _mapConsole.SetGlyph(i, j, 219);
                            break;
                    }
                }
            }
        }

        public void DrawMapPoint(int x, int y)
        {
            var wMap = Adventure.Map.TileMap;
            switch (wMap[x, y])
            {
                case StoneTile _:
                    _mapConsole[x, y].CopyAppearanceFrom(StoneTileAppearance);
                    _mapConsole.SetGlyph(x, y, 46);
                    break;
                case StoneWall _:
                    _mapConsole[x, y].CopyAppearanceFrom(StoneWallAppearance);
                    _mapConsole.SetGlyph(x, y, 35);
                    break;
                default:
                    break;

            }
        }

        public void DrawEntity(IAdventureEntity adventureEntity)
        {
            int glyph;
            Color colour;

            switch (adventureEntity)
            {
                case Mogwai _:
                    glyph = 1;
                    colour = Color.LimeGreen;
                    break;
                case Monster _:
                    glyph = 64;
                    colour = Color.Red;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // TODO: rotating symbols for multiple mogwais
            var animated = new Animated("default", 1, 1, AdventureFont);
            var frame = animated.CreateFrame();
            frame[0].Glyph = glyph;
            frame[0].Foreground = colour;

            var pos = adventureEntity.Coordinate;
            var entity = new ConsoleEntity(animated)
            {
                Position = new Point(pos.X, pos.Y)
            };

            _mapConsole.Children.Add(entity);
            _entities.Add(adventureEntity.AdventureEntityId, entity);
        }

        public void UpdateGame()
        {
            if (!_mapConsole.IsVisible || LastUpdate.Add(GameSpeed) >= DateTime.Now)
                return;

            LastUpdate = DateTime.Now;

            // next frame
            if (Adventure.AdventureLogs.Count == 0 && Adventure.HasNextFrame())
            {
                Adventure.NextFrame();
            }

            if (!Adventure.AdventureLogs.TryDequeue(out var log))
                return;

            // redraw map
            DrawExploMap();
            //DrawMap();
            DrawFoV(log.SourceFovCoords);

            // stats
            _statsConsole.Print(2, 0, Adventure.AdventureStats[AdventureStats.Explore].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 1, Adventure.AdventureStats[AdventureStats.Monster].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 2, Adventure.AdventureStats[AdventureStats.Boss].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 3, Adventure.AdventureStats[AdventureStats.Treasure].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 4, Adventure.AdventureStats[AdventureStats.Portal].ToString("0%").PadLeft(4), Color.Gold);

            switch (log.Type)
            {
                case AdventureLog.LogType.Info:
                    break;
                case AdventureLog.LogType.Move:
                    MoveEntity(_entities[log.Source], log.TargetCoord);
                    break;
                case AdventureLog.LogType.Attack:
                    AttackEntity(log.TargetCoord);
                    break;
                case AdventureLog.LogType.Entity:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawFoV(HashSet<Coord> fovCoords)
        {
            foreach (var fovCoord in fovCoords)
            {
                _mapConsole[fovCoord.X, fovCoord.Y].Background = Color.Lerp(Color.DarkGray, Color.Black, 0.75f);
            }
        }

        private void MoveEntity(ConsoleEntity entity, Coord destination)
        {
            entity.Position = new Point(destination.X, destination.Y);
        }

        private void AttackEntity(Coord targetCoord)
        {
            var effect = new Animated("default", 1, 1, AdventureFont);

            var frame = effect.CreateFrame();
            effect.CreateFrame();
            frame[0].Glyph = 15;

            effect.AnimationDuration = 1;
            effect.Start();

            var entity = new ConsoleEntity(effect) { Position = new Point(targetCoord.X, targetCoord.Y) };
            _mapConsole.Children.Add(entity);
            
        }
    }
}
