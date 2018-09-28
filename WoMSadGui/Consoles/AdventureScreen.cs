using System;
using System.Collections.Generic;
using GoRogue;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Surfaces;
using WoMFramework.Game.Generator;
using WoMWallet.Node;
using WoMFramework.Game.Model.Dungeon;
using WoMFramework.Game.Model.Mogwai;
using WoMFramework.Game.Model.Monster;
using Console = SadConsole.Console;
using ConsoleEntity = SadConsole.Entities.Entity;

namespace WoMSadGui.Consoles
{
    public class AdventureScreen : Console
    {
        private static readonly Cell StoneTileAppearance = new Cell(Color.Black, Color.DarkGray, 46);
        private static readonly Cell StoneWallAppearance = new Cell(Color.Black, Color.DarkGray, 35);
        private static readonly Font AdventureFont;

        private readonly MogwaiController _controller;
        private readonly MogwaiKeys _mogwaiKeys;
        private readonly Mogwai _mogwai;
        private readonly Dictionary<int, ConsoleEntity> _entities = new Dictionary<int, ConsoleEntity>();

        public static TimeSpan GameSpeed = TimeSpan.FromSeconds(0.5);
        public DateTime LastUpdate;

        public Adventure Adventure { get; }

        // TODO: EntityManager

        static AdventureScreen()
        {
            FontMaster fm = Global.LoadFont("Cheepicus12.font");
            AdventureFont = fm.GetFont(Font.FontSizes.Two);
        }

        public AdventureScreen(MogwaiController mogwaiController, MogwaiKeys mogwaiKeys, int width, int height) : base(width, height)
        {
            Font = AdventureFont;

            _controller = mogwaiController;
            _mogwaiKeys = _controller.CurrentMogwaiKeys ?? _controller.TestMogwaiKeys();
            _mogwai = _mogwaiKeys.Mogwai;

            Adventure = new SimpleDungeon(_mogwai.CurrentShift);

            DrawMap();
        }

        public static AdventureScreen Test(MogwaiController mogwaiController)
        {
            //var dungeon = new SimpleDungeon(mogwaiController.CurrentMogwai.CurrentShift);

            var console = new AdventureScreen(mogwaiController, null, 20, 20);


            var dungeon = console.Adventure as SimpleDungeon;
            var mog = console._mogwai;
            mog.Adventure = dungeon;
            mog.Evolve(out _);

            // Draw entities (Mogwais, Monsters, etc.)
            var map = dungeon.Map;
            foreach (var entity in map.GetEntities())
                if (!entity.IsStatic)
                    console.DrawEntity(entity);

            Global.CurrentScreen.Children.Clear();
            Global.CurrentScreen.Children.Add(console);
            Global.FocusedConsoles.Set(console);

            console.LastUpdate = DateTime.Now;

            for (int i = 0; i < 15; i ++)
                foreach (var entity in map.GetEntities())
                    entity.MoveArbitrary();

            return console;
        }

        public void DrawMap()
        {
            var map = Adventure.Map;

            // TODO: Viewport
            Resize(map.Width, map.Height, true);

            var wMap = map.WalkabilityMap;
            for (int i = 0; i < wMap.Width; i++)
            for (int j = 0; j < wMap.Height; j++)
                this[i, j].CopyAppearanceFrom(wMap[i, j] ? StoneTileAppearance : StoneWallAppearance);
        }

        public void DrawEntity(IAdventureEntity adventureEntity)
        {
            int glyph;
            Color colour;

            switch (adventureEntity)
            {
                case Mogwai _:
                    glyph = 1;
                    colour = Color.Green;
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

            Coord pos = adventureEntity.Coordinate;
            var entity = new ConsoleEntity(animated) {Position = new Point(pos.X, pos.Y)};

            Children.Add(entity);
            _entities.Add(adventureEntity.AdventureEntityId, entity);
        }

        public void UpdateGame()
        {
            if (LastUpdate.Add(GameSpeed) >= DateTime.Now) return;

            LastUpdate = DateTime.Now;
            if (!Adventure.AdventureLogs.TryDequeue(out var log))
                return;
            switch (log.Type)
            {
                case AdventureLog.LogType.Info:
                    break;
                case AdventureLog.LogType.Move:
                    MoveEntity(_entities[log.Source], log.TargetCoord);
                    break;
                case AdventureLog.LogType.Attack:
                    break;
                case AdventureLog.LogType.Entity:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void MoveEntity(ConsoleEntity entity, Coord destination)
        {
            entity.Position = new Point(destination.X, destination.Y);
        }
    }
}
