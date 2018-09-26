using System.Collections.Generic;
using WoMFramework.Game.Model;
using WoMFramework.Game.Random;

namespace WoMFramework.Game.Combat
{
    internal class Brawler
    {
        public bool IsHero { get; set; } = false;
        public Entity Entity { get; }
        public int InititativeValue { get; set; }
        public Dice Dice { get; set; }
        public List<Entity> Enemies { get; set; }

        public Brawler(Entity entity)
        {
            Entity = entity;
        }

    }
}
