using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMFramework.Game.Model
{
    public abstract class Wall
    {
        public Tile Parent;

        public abstract bool IsBlocked { get; set; }

        public abstract bool Interact(Mogwai mogwai);
    }

    public class Door : Wall
    {
        public Tile Inside;
        public Tile Outside;
        public override bool IsBlocked { get; set; } = false;

        public override bool Interact(Mogwai mogwai)
        {
            throw new NotImplementedException();
        }
    }

    public class StoneWall : Wall
    {
        public override bool IsBlocked { get; set; } = true;

        public override bool Interact(Mogwai mogwai)
        {
            throw new NotImplementedException();
        }
    }
}
