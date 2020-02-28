namespace WoMSadGui.Consoles
{
    using GoRogue;
    using Microsoft.Xna.Framework;
    using SadConsole;
    using SadConsole.Surfaces;
    using Specific;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using WoMFramework.Game;
    using WoMFramework.Game.Enums;
    using WoMFramework.Game.Generator;
    using WoMFramework.Game.Generator.Dungeon;
    using WoMFramework.Game.Model.Actions;
    using WoMFramework.Game.Model.Learnable;
    using WoMFramework.Game.Model.Mogwai;
    using WoMFramework.Game.Model.Monster;
    using WoMWallet.Node;
    using Console = SadConsole.Console;
    using ConsoleEntity = SadConsole.Entities.Entity;

    public class CustomAdventure : MogwaiConsole
    {
        private static readonly Cell UnknownAppearance = new Cell(new Color(25, 25, 25), Color.Black, 219);
        private static readonly Cell UnclearAppearance = new Cell(new Color(50, 205, 50), new Color(50, 205, 50, 150), 219);
        private static readonly Cell StoneTileAppearance = new Cell(Color.DarkGray, Color.Black, 46);
        private static readonly Cell StoneWallAppearance = new Cell(Color.DarkGray, Color.Black, 35);

        private readonly Font _adventureFont;

        private readonly MogwaiController _controller;
        private readonly MogwaiKeys _mogwaiKeys;
        private readonly Mogwai _mogwai;

        private readonly Console _mapConsole;
        private readonly int _mapViewWidth = 45;
        private readonly int _mapViewHeight = 22;


        private readonly Console _statsConsole;

        private readonly Dictionary<int, ConsoleEntity> _entities = new Dictionary<int, ConsoleEntity>();

        public static TimeSpan GameSpeed = TimeSpan.Zero;

        public static TimeSpan ActionDelay = TimeSpan.FromSeconds(0.05);

        public static TimeSpan CombatActionDelay = TimeSpan.FromSeconds(0.5);

        private readonly SadConsole.Entities.EntityManager _entityManager;
        public DateTime LastUpdate;

        public Adventure Adventure { get; private set; }

        public CustomAdventure(MogwaiController mogwaiController, MogwaiKeys mogwaiKeys, int width, int height) : base("Adventure", "", width, height)
        {
            _controller = mogwaiController;
            _mogwaiKeys = mogwaiKeys;
            _mogwai = _mogwaiKeys.Mogwai;

            _adventureFont = Global.LoadFont("Phoebus16.font").GetFont(Font.FontSizes.One);

            //_mapConsole = new Console(75, 75) { Position = new Point(17, 2) };
            _mapConsole = new Console(100, 100)
            {
                Position = new Point(-24, 0),
                Font = _adventureFont,
                IsVisible = false
            };
            Children.Add(_mapConsole);

            _entityManager = new SadConsole.Entities.EntityManager { Parent = _mapConsole };

            _statsConsole = new MogwaiConsole("stats", "", 21, 6) { Position = new Point(70, 16) };
            _statsConsole.Fill(DefaultForeground, new Color(10, 10, 10, 230), null);
            Children.Add(_statsConsole);
        }

        public bool IsStarted()
        {
            return _mapConsole.IsVisible;
        }

        public void Start(Adventure adventure)
        {
            _mapConsole.ViewPort = new Microsoft.Xna.Framework.Rectangle(0, 0, _mapViewWidth, _mapViewHeight);
            _mapConsole.Children.Clear();
            _entities.Clear();

            Adventure = adventure;

            DrawExploMap();

            _statsConsole.Print(7, 0, "Rounds", Color.MonoGameOrange);
            _statsConsole.Print(7, 1, "Exploration", Color.Gainsboro);
            _statsConsole.Print(7, 2, "Monster", Color.Gainsboro);
            _statsConsole.Print(7, 3, "Boss", Color.Gainsboro);
            _statsConsole.Print(7, 4, "Treasure", Color.Gainsboro);
            _statsConsole.Print(7, 5, "Portal", Color.Gainsboro);

            // Draw entities (Mogwais, Monsters, etc.)
            foreach (AdventureEntity entity in Adventure.Map.Entities)
            {
                // TODO this has to be analyzed !!!
                if (entity == null)
                {
                    continue;
                }

                if (!entity.IsStatic)
                {
                    DrawEntity(entity);
                }
            }

            _mapConsole.IsVisible = true;
            LastUpdate = DateTime.Now;
        }

        public void Stop()
        {
            _mapConsole.IsVisible = false;

            _mapConsole.Children.Clear();
            _entities.Clear();
            _statsConsole.Clear();
        }

        public void DrawExploMap()
        {
            GoRogue.MapViews.ArrayMap<int> wMap = Adventure.Map.ExplorationMap;
            for (var i = 0; i < wMap.Width; i++)
            {
                for (var j = 0; j < wMap.Height; j++)
                {
                    switch (wMap[i, j])
                    {
                        case 0:
                            Cell cell = UnknownAppearance;
                            _mapConsole[i, j].CopyAppearanceFrom(cell);
                            _mapConsole.SetGlyph(i, j, 176);
                            break;
                        case 1:
                        case 2:
                        case -1:
                            DrawMapPoint(i, j, false);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void DrawMapPoint(int x, int y, bool isUnclear)
        {
            GoRogue.MapViews.ArrayMap<Tile> wMap = Adventure.Map.TileMap;
            switch (wMap[x, y])
            {
                case StoneTile _:
                    _mapConsole[x, y].CopyAppearanceFrom(isUnclear ? UnclearAppearance : StoneTileAppearance);
                    _mapConsole.SetGlyph(x, y, 46);//43);
                    break;

                case StoneWall _:
                    _mapConsole[x, y].CopyAppearanceFrom(isUnclear ? UnclearAppearance : StoneWallAppearance);
                    _mapConsole.SetGlyph(x, y, 245); //35);
                    break;

                default:
                    break;
            }
        }

        public void DrawEntity(AdventureEntity adventureEntity)
        {
            int glyph;
            Color color;

            switch (adventureEntity)
            {
                case Mogwai _:
                    glyph = 1;
                    color = Color.DarkOrange;
                    break;
                case Monster _:
                    glyph = 135; //64;
                    color = Color.SandyBrown;
                    break;
                case Chest _:
                    glyph = 146; //64;
                    color = Color.Pink;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // TODO: rotating symbols for multiple mogwais
            var defaultAnim = new Animated("default", 1, 1, _adventureFont);
            BasicNoDraw frame = defaultAnim.CreateFrame();
            frame[0].Glyph = glyph;
            frame[0].Foreground = color;

            Coord pos = adventureEntity.Coordinate;
            var entity = new ConsoleEntity(defaultAnim)
            {
                Position = new Point(pos.X, pos.Y),
            };

            // damage animation
            var defAnimated = new Animated("default", 1, 1, _adventureFont);
            var animEntity = new ConsoleEntity(defAnimated);
            var damageAnim = new Animated("damage", 1, 1, _adventureFont) { AnimationDuration = 1 };
            BasicNoDraw damageFrame = damageAnim.CreateFrame();
            damageAnim.CreateFrame();
            damageFrame[0].Glyph = 15;
            damageFrame[0].Foreground = Color.Red;
            animEntity.Animations.Add("damage", damageAnim);

            // add animation entity
            entity.Children.Add(animEntity);

            // TODO change this ... to a more appropriate handling
            // do not revive ... dead shapes
            if (adventureEntity is Combatant combatant && combatant.IsDead)
            {
                DiedEntity(entity);
            }

            entity.IsVisible = false;

            _entities.Add(adventureEntity.AdventureEntityId, entity);
            _entityManager.Entities.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
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

            if (!Adventure.AdventureLogs.TryDequeue(out AdventureLog adventureLog))
            {
                GameSpeed = TimeSpan.Zero;
                return;
            }

            if (_mogwai.CombatState != CombatState.None)
            {
                // combat gamespeed
                GameSpeed = CombatActionDelay;
            }
            else
            {
                // gamespeed of unseen entities have no delay
                GameSpeed = _mogwai.CanSee(Adventure.Map.Entities.FirstOrDefault(p => p.AdventureEntityId == adventureLog.Source)) ? ActionDelay : TimeSpan.Zero;
            }

            // redraw map
            DrawExploMap();

            //DrawMap();
            adventureLog.SourceFovCoords?.ToList().ForEach(p => _mapConsole[p.X, p.Y].Background = Color.Lerp(Color.Yellow, Color.Black, 0.50f));

            // stats
            _statsConsole.Print(2, 0, Adventure.GetRound.ToString().PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 1, Adventure.AdventureStats[AdventureStats.Explore].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 2, Adventure.AdventureStats[AdventureStats.Monster].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 3, Adventure.AdventureStats[AdventureStats.Boss].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 4, Adventure.AdventureStats[AdventureStats.Treasure].ToString("0%").PadLeft(4), Color.Gold);
            _statsConsole.Print(2, 5, Adventure.AdventureStats[AdventureStats.Portal].ToString("0%").PadLeft(4), Color.Gold);

            switch (adventureLog.Type)
            {
                case LogType.Info:
                    //Adventure.Enqueue(AdventureLog.Info(this, target, ActivityLog.Create(ActivityLog.ActivityType.Attack, ActivityLog.ActivityState.Success, new int[] { damage, criticalDamage, (int)DamageType.Weapon }, weaponAttack)));
                    var source = Adventure.Entities[adventureLog.Source];
                    var target = Adventure.Entities[adventureLog.Target];
                    var message = "";
                    var activityLog = adventureLog.ActivityLog;
                    switch (activityLog.Type)
                    {
                        case ActivityLog.ActivityType.Cast:
                            var spell = activityLog.ActivityObject as Spell;
                            message = $"{Coloring.Name(source.Name)} cast " +
                                      $"{Coloring.Violet(spell.Name)} on {Coloring.Name(target.Name)} roll " +
                                      $"{activityLog.Numbers[0].ToString()}:";
                            switch (activityLog.State)
                            {
                                case ActivityLog.ActivityState.Success:
                                    message = $"{message} {Coloring.Green("success")}!";
                                    break;
                                case ActivityLog.ActivityState.Fail:
                                    message = $"{message} {Coloring.Red("failed")}!";
                                    break;
                                case ActivityLog.ActivityState.Init:
                                    message = $"{Coloring.Name(source.Name)} starting to cast {Coloring.Violet(spell.Name)}.";
                                    break;
                            }
                            break;

                        case ActivityLog.ActivityType.Attack:
                            var weaponAttack = activityLog.ActivityObject as WeaponAttack;
                            message = $"{Coloring.Name(source.Name)} [{Coloring.Orange(activityLog.Numbers[0].ToString())}] " +
                                          $"{weaponAttack.GetType().Name.ToLower()}[{Coloring.Gainsboro(weaponAttack.ActionType.ToString().Substring(0, 1))}] " +
                                          $"{Coloring.Name(target.Name)} with {Coloring.DarkName(weaponAttack.Weapon.Name)} roll " +
                                          $"{Coloring.Attack(activityLog.Numbers[2] > 0 ? "critical" : activityLog.Numbers[1].ToString())}:";
                            switch (activityLog.State)
                            {
                                case ActivityLog.ActivityState.Success:
                                    message = $"{message} {Coloring.Green("hit for")} {Coloring.DoDmg(activityLog.Numbers[3])}[{activityLog.Numbers[4]}] {Coloring.Green("damage!")}";
                                    break;
                                case ActivityLog.ActivityState.Fail:
                                    message = $"{message} {Coloring.Red("failed")}!";
                                    break;
                                case ActivityLog.ActivityState.Init:
                                    message = $"{Coloring.Name(source.Name)} [{Coloring.Orange(activityLog.Numbers[0].ToString())}]: Initiating Attack";
                                    break;
                            }
                            break;

                        case ActivityLog.ActivityType.Heal:
                            message = $"{Coloring.Name(source.Name)} restores {Coloring.GetHeal(activityLog.Numbers[0])} HP from {((HealType)activityLog.Numbers[1]).ToString()} healing.";
                            break;

                        case ActivityLog.ActivityType.Damage:
                            message = $"{Coloring.Name(source.Name)} suffers {Coloring.GetDmg(activityLog.Numbers[0])} HP from {((DamageType)activityLog.Numbers[1]).ToString()} damage.";
                            break;

                        case ActivityLog.ActivityType.HealthState:
                            var healthState = (HealthState)activityLog.Numbers[0];
                            switch (healthState)
                            {
                                case HealthState.Dead:
                                    message = $"{Coloring.Name(source.Name)} has died, may its soul rest in peace. Its health state is {Coloring.Red(healthState.ToString())}.";
                                    break;
                                case HealthState.Dying:
                                    message = $"{Coloring.Name(source.Name)} got a deadly hit, health state is {Coloring.Red(healthState.ToString())}.";
                                    break;
                                case HealthState.Disabled:
                                    message = $"{Coloring.Name(source.Name)} got a deadly hit, health state is {Coloring.Red(healthState.ToString())}.";
                                    break;
                            }
                            break;

                        case ActivityLog.ActivityType.Loot:
                            message = $"{Coloring.Name(source.Name)} is looting {Coloring.DarkGrey(target.Name)}.";
                            break;

                        case ActivityLog.ActivityType.Treasure:
                            if (activityLog.State == ActivityLog.ActivityState.Success)
                            {
                                message = $"{Coloring.DarkGrey(_mogwai.Name)} found a treasure!";
                            }
                            else
                            {
                                message = $"{Coloring.DarkGrey(_mogwai.Name)} found nothing!";
                            }
                            break;

                        case ActivityLog.ActivityType.Gold:
                            message = $"{Coloring.DarkGrey(_mogwai.Name)} got {Coloring.Gold(activityLog.Numbers[0])} gold";
                            break;

                        case ActivityLog.ActivityType.LevelClass:
                            message = Coloring.LevelUp($"You feel the power of the {((ClassType)activityLog.Numbers[0])}'s!");
                            break;

                        case ActivityLog.ActivityType.Exp:
                            message = activityLog.ActivityObject == null
                                ? $"You just earned +{Coloring.Exp(activityLog.Numbers[0])} experience!"
                                : $"The {Coloring.Name((activityLog.ActivityObject as Monster).Name)} gave you +{Coloring.Exp(activityLog.Numbers[0])}!";
                            break;
                        case ActivityLog.ActivityType.Level:
                            message = $"{Coloring.LevelUp("Congratulations he just made the")} {Coloring.Green(activityLog.Numbers[0].ToString())} {Coloring.LevelUp("th level!")}";
                            break;
                    }
                    ((PlayScreen)Parent).PushLog(LogType.Info, message);

                    break;
                case LogType.Move:
                    MoveEntity(_entities[adventureLog.Source], adventureLog.TargetCoord);
                    break;
                case LogType.Attack:
                    AttackEntity(_entities[adventureLog.Target], adventureLog.TargetCoord);
                    break;
                case LogType.Died:
                    DiedEntity(_entities[adventureLog.Source]);
                    break;
                case LogType.Entity:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            foreach (AdventureEntity entity in Adventure.Map.Entities)
            {
                if (entity.IsStatic)
                {
                    continue;
                }

                ConsoleEntity consoleEntity = _entities[entity.AdventureEntityId];

                consoleEntity.IsVisible =
                    (_mogwai.CanSee(entity) || entity is Combatant combatant && combatant.IsDead) &&
                    _mapConsole.ViewPort.Contains(consoleEntity.Position);
            }
        }

        private void MoveEntity(ConsoleEntity entity, Coord destination)
        {
            entity.Position = new Point(destination.X, destination.Y);
            _mapConsole.ViewPort = new Microsoft.Xna.Framework.Rectangle(destination.X - _mapViewWidth / 2, destination.Y - _mapViewHeight / 2, _mapViewWidth, _mapViewHeight);
            _entityManager.Sync();
        }

        private void AttackEntity(ConsoleEntity entity, Coord targetCoord)
        {
            var animationEntity = entity.Children.First(p => p is ConsoleEntity) as ConsoleEntity;
            animationEntity.Animation = animationEntity.Animations["damage"];
            animationEntity.Animation.Restart();
        }

        private void DiedEntity(ConsoleEntity entity)
        {
            var animated = new Animated("default", 1, 1, _adventureFont);
            BasicNoDraw frame = animated.CreateFrame();
            frame[0].Glyph = 143;
            frame[0].Foreground = Color.Gainsboro;
            entity.Animation = animated;
        }
    }
}
