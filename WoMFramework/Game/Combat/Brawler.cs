using System;
using System.Collections.Generic;
using System.Text;
using WoMFramework.Game.Model;

namespace WoMFramework.Game.Combat
{
    class Brawler
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
