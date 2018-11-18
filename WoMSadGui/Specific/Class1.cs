using System;
using System.Collections.Generic;
using System.Text;
using SadConsole;
using SadConsole.Entities;
using SadConsole.Surfaces;

namespace WoMSadGui.Specific
{
    public class MogwaiEntity : Entity
    {
        public MogwaiEntity(int width, int height) : base(1, 1)
        {
        }

        public MogwaiEntity(int width, int height, Font font) : base(width, height, font)
        {
        }

        public MogwaiEntity(Animated animation) : base(animation)
        {
        }
    }
}
