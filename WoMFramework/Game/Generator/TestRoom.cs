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
                _simpleFight.Prepare(mogwai, shift);
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