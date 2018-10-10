using System;
using System.Collections.Generic;
using System.Linq;
using GoRogue;
using GoRogue.MapViews;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using SadConsole.Themes;
using WoMFramework.Game.Generator;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Actions;
using WoMWallet.Node;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using Console = SadConsole.Console;
using ConsoleEntity = SadConsole.Entities.Entity;

namespace WoMSadGui.Consoles
{
    public class AdventureConsole : MogwaiConsole
    {
        private static readonly Cell StoneTileAppearance = new Cell(Color.DarkGray, Color.Black, 46);
        private static readonly Cell StoneWallAppearance = new Cell(Color.DarkGray, Color.Black, 35);

        private readonly MogwaiController _controller;
        private readonly MogwaiKeys _mogwaiKeys;
        private readonly Mogwai _mogwai;

        private Console _mapConsole;

        private readonly Dictionary<int, ConsoleEntity> _entities = new Dictionary<int, ConsoleEntity>();

        public static TimeSpan GameSpeed = TimeSpan.FromSeconds(0.5);
        public DateTime LastUpdate;

        public Adventure Adventure { get; private set; }

        public Dictionary<string, int> WallTypeGlyph = new Dictionary<string, int>();

        public AdventureConsole(MogwaiController mogwaiController, MogwaiKeys mogwaiKeys, int width, int height) : base("Custom", "", width, height)
        {
            _controller = mogwaiController;
            _mogwaiKeys = mogwaiKeys;
            _mogwai = _mogwaiKeys.Mogwai;

            _mapConsole = new Console(Width-2, Height) { Position = new Point(1, 0) };
            Children.Add(_mapConsole);

            FillWallTypeGlyph();
        }

        private void FillWallTypeGlyph()
        {
            WallTypeGlyph.Add("111" + "111" + "111", 219);
            WallTypeGlyph.Add("000" + "010" + "000", 219);
            WallTypeGlyph.Add("010" + "010" + "010", 179);
            WallTypeGlyph.Add("111" + "110" + "110", 179);
            WallTypeGlyph.Add("110" + "110" + "110", 179);
            WallTypeGlyph.Add("110" + "110" + "111", 179);
            WallTypeGlyph.Add("111" + "011" + "011", 179);
            WallTypeGlyph.Add("011" + "011" + "011", 179);
            WallTypeGlyph.Add("011" + "011" + "111", 179);
            WallTypeGlyph.Add("000" + "111" + "000", 196);
            WallTypeGlyph.Add("111" + "111" + "000", 196);
            WallTypeGlyph.Add("111" + "111" + "100", 196);
            WallTypeGlyph.Add("111" + "111" + "001", 196);
            WallTypeGlyph.Add("000" + "111" + "111", 196);
            WallTypeGlyph.Add("100" + "111" + "111", 196);
            WallTypeGlyph.Add("001" + "111" + "111", 196);
            WallTypeGlyph.Add("010" + "110" + "010", 180);
            WallTypeGlyph.Add("010" + "010" + "111", 193);
            WallTypeGlyph.Add("111" + "010" + "010", 194);
            WallTypeGlyph.Add("010" + "011" + "010", 195);
            WallTypeGlyph.Add("010" + "111" + "010", 197);
            WallTypeGlyph.Add("000" + "011" + "010", 218);
            WallTypeGlyph.Add("111" + "111" + "110", 218);
            WallTypeGlyph.Add("011" + "111" + "111", 217);
            WallTypeGlyph.Add("010" + "110" + "000", 217);
            WallTypeGlyph.Add("010" + "011" + "000", 192);
            WallTypeGlyph.Add("110" + "111" + "111", 192);
            WallTypeGlyph.Add("000" + "110" + "010", 191);
            WallTypeGlyph.Add("111" + "111" + "011", 191);
        }

        public void Start(Adventure adventure)
        {
            _mapConsole.Children.Clear();
            _entities.Clear();

            Adventure = adventure;

            DrawMap();

            // Draw entities (Mogwais, Monsters, etc.)
            foreach (var entity in Adventure.Map.GetEntities())
                if (!entity.IsStatic)
                    DrawEntity(entity);

            LastUpdate = DateTime.Now;
        }

        public void DrawMap()
        {
            // TODO: Viewport
            //Resize(Adventure.Map.Width, Adventure.Map.Height, true);

            var wMap = Adventure.Map.TileMap;
            for (var i = 0; i < wMap.Width; i++)
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
                            //SetGlyph(i, j, GetGlyphFromSurrounding(i,j, wMap));
                            _mapConsole.SetGlyph(i, j, 35);
                            break;
                        default:
                            break;

                    }
                }
        }

        private int GetGlyphFromSurrounding(int x, int y, ArrayMap<Tile> map)
        {
            if (map[x, y].IsSolid)
            {
                string solidMap = string.Empty;
                for (int yr = y - 1; yr <= y + 1; yr++)
                    for (int xr = x - 1; xr <= x + 1; xr++)
                        try { solidMap += !map[xr, yr].IsReachable ? "1" : "0";}
                            catch (Exception) { solidMap += "1"; }
                return WallTypeGlyph[solidMap];
            }

            return 46;
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
            var animated = new Animated("default", 1, 1);
            var frame = animated.CreateFrame();
            frame[0].Glyph = glyph;
            frame[0].Foreground = colour;

            var pos = adventureEntity.Coordinate;
            var entity = new ConsoleEntity(animated) { Position = new Point(pos.X, pos.Y) };

            _mapConsole.Children.Add(entity);
            _entities.Add(adventureEntity.AdventureEntityId, entity);
        }

        public void UpdateGame()
        {
            if (LastUpdate.Add(GameSpeed) >= DateTime.Now) return;

            LastUpdate = DateTime.Now;

            if (!Adventure.AdventureLogs.TryDequeue(out var log))
                return;

            // redraw map
            DrawMap();
            DrawFoV(log.SourceFovCoords);

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
            var effect = new Animated("default", 1, 1);

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
