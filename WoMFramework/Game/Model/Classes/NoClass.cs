using WoMFramework.Game.Enums;

namespace WoMFramework.Game.Model
{
    public class NoClass : Classes
    {
        public NoClass() : base(ClassType.None)
        {
        }

        public override void ClassLevelUp()
        {
            base.ClassLevelUp();
        }
    }
}
