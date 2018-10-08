using WoMFramework.Game.Combat;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;
using WoMFramework.Game.Model.Mogwai;

namespace WoMFramework.Game.Generator
{
    public class TestRoom : Adventure
    {
        private readonly SimpleBrawl _simpleFight;

        public TestRoom(SimpleBrawl simpleFight)
        {
            this._simpleFight = simpleFight;
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

            AdventureState = AdventureState.Completed;
        }
    }
}