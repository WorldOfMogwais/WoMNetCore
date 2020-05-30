namespace WoMFramework.Game.Home
{
    using Interaction;
    using Random;

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
