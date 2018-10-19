using WoMFramework.Game.Interaction;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Home
{
    public class HomeTown
    {
        private Shift _shift;

        public Shop Shop { get; }

        public Dice Dice { get; }

        public HomeTown(Shift shift)
        {
            _shift = shift;
            Dice = new Dice(shift, 40337035);

            Shop = new Shop(shift);
        }
    }
}
