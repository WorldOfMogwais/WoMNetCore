using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public abstract class Adventure
    {
        public AdventureState AdventureState { get; set; }

        public bool IsActive => AdventureState == AdventureState.Creation || AdventureState == AdventureState.Running;

        public Adventure()
        {
            AdventureState = AdventureState.Creation;
        }

        public abstract void NextStep(Mogwai mogwai, Shift shift);
    }
}