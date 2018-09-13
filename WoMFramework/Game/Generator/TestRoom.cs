using WoMFramework.Game.Combat;
using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public class TestRoom : Adventure
    {
        private SimpleBrawl simpleFight;

        public TestRoom(SimpleBrawl simpleFight)
        {
            this.simpleFight = simpleFight;
        }

        public override void NextStep(Mogwai mogwai, Shift shift)
        {
            if (AdventureState == AdventureState.Preparation)
            {
                simpleFight.Prepare(mogwai, shift);
                AdventureState = AdventureState.Running;
            }

            if (!simpleFight.Run())
            {
                AdventureState = AdventureState.Failed;
                return;
            }

            AdventureState = AdventureState.Completed;
        }
    }
}