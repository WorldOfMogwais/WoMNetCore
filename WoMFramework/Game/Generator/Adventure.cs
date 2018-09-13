using WoMFramework.Game.Interaction;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Generator
{
    public abstract class Adventure
    {
        public AdventureState AdventureState { get; set; }

        public bool IsActive => AdventureState == AdventureState.Preparation 
                             || AdventureState == AdventureState.Running;

        public Adventure()
        {
            AdventureState = AdventureState.Preparation;
        }

        public abstract void NextStep(Mogwai mogwai, Shift shift);
    }
}