using System.Collections.Generic;

namespace WoMFramework.Game.Model
{
    public partial class Monsters
    {

        public static List<Monster> Animals { get; set; }

        public Monsters()
        {
            Animals = new List<Monster>()
            {
                Rat,
                Wolf
            };
        }
    }
}
