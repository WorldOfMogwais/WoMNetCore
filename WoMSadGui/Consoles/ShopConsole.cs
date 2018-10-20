using System.Linq;
using Microsoft.Xna.Framework;
using WoMFramework.Game.Model.Mogwai;

namespace WoMSadGui.Consoles
{
    internal class ShopConsole : MogwaiConsole
    {
        private readonly Mogwai _mogwai;

        public ShopConsole(Mogwai mogwai, int width, int height) : base("Home Town Shop", "", width, height)
        {
            _mogwai = mogwai;
            Init();
        }

        public void Init()
        {
            var str = string.Join(';',_mogwai.HomeTown.Shop.Inventory.Select(p => p.Name).ToArray());
            Print(0,0, str);
        }
    }
}