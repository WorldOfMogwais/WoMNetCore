namespace WoMFramework.Game.Model
{
    public static class Coloring
    {
        public static string Name(string name)
        {
            return $"[c:r f:cyan]{name}[c:u]";
        }

        public static string DarkName(string name)
        {
            return $"[c:r f:darkcyan]{name}[c:u]";
        }

        public static string Armor(int value)
        {
            return $"[c:r f:silver]{value.ToString()}[c:u]";
        }

        public static string Attack(string name)
        {
            return $"[c:r f:yellow]{name}[c:u]";
        }

        public static string Hitpoints(int value)
        {
            return $"[c:r f:limegreen]{value.ToString()}[c:u]";
        }

        public static string GetHeal(int value)
        {
            return $"[c:r f:lime]{value.ToString()}[c:u]";
        }

        public static string GetDmg(int value)
        {
            return $"[c:r f:red]{value.ToString()}[c:u]";
        }

        public static string DoDmg(int value)
        {
            return $"[c:r f:orange]{value.ToString()}[c:u]";
        }

        public static string DoCritDmg(int value)
        {
            return $"[c:r f:darkorange]{value.ToString()}[c:u]";
        }

        public static string Green(string name)
        {
            return $"[c:r f:limegreen]{name}[c:u]";
        }
        public static string Red(string name)
        {
            return $"[c:r f:red]{name}[c:u]";
        }
        public static string Orange(string name)
        {
            return $"[c:r f:orange]{name}[c:u]";
        }

        public static string Exp(double exp)
        {
            return Green(exp.ToString());
        }

        public static string LevelUp(string name)
        {
            return $"[c:r f:violet]{name}[c:u]";
        }
    }
}