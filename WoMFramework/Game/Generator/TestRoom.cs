using System.Collections.Generic;
using System.Linq;
using GoRogue;
using WoMFramework.Game.Combat;
using WoMFramework.Game.Generator.Dungeon;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;

namespace WoMFramework.Game.Generator
{
    public class TestRoom : Adventure
    {
        private readonly SimpleCombat _simpleFight;

        public TestRoom(SimpleCombat simpleFight)
        {
            _simpleFight = simpleFight;
            Map = new Map(10, 10, this);
        }

        public override Map Map { get; set; }

        public override void NextStep(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                var coords = new List<Coord>();
                _simpleFight.Prepare(mogwai, shift);
                var hero = _simpleFight.Heroes.First();
                Map.AddEntity(hero, 4, 4);
                coords.AddRange(new RadiusAreaProvider(hero.Coordinate, 1, Radius.CIRCLE)
                    .CalculatePositions().ToList());

                foreach (var entity in _simpleFight.Monsters)
                {
                    var getAPlace = coords[1];
                    Map.AddEntity(entity, getAPlace.X, getAPlace.Y);
                    coords.Remove(getAPlace);
                }

                AdventureState = AdventureState.Running;
            }

            if (!_simpleFight.Run())
            {
                AdventureState = AdventureState.Failed;
                return;
            }

            EvaluateAdventureState();
        }

        public override void EvaluateAdventureState()
        {
            AdventureState = AdventureState.Completed;
        }
    }
}