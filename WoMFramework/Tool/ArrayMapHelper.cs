using GoRogue.MapViews;

namespace WoMFramework.Tool
{
    public enum RotateType
    {
        DEG90, DEG180, DEG270
    }

    public static class ArrayMapHelper
    {
        public static ArrayMap<T> RotateArrayMap<T>(ArrayMap<T> arrayMap, RotateType rotateType)
        {
            ArrayMap<T> newPattern = null;
            switch (rotateType)
            {
                case RotateType.DEG90:
                    newPattern = new ArrayMap<T>(arrayMap.Height, arrayMap.Width);
                    foreach(var pos in arrayMap.Positions())
                    {
                        newPattern[pos.Y, arrayMap.Width - 1 - pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                case RotateType.DEG180:
                    newPattern = new ArrayMap<T>(arrayMap.Width, arrayMap.Height);
                    foreach (var pos in arrayMap.Positions())
                    {
                        newPattern[arrayMap.Width - 1 - pos.Y, arrayMap.Height - 1 - pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                case RotateType.DEG270:
                    newPattern = new ArrayMap<T>(arrayMap.Height, arrayMap.Width);
                    foreach (var pos in arrayMap.Positions())
                    {
                        newPattern[arrayMap.Height - 1 - pos.Y, pos.X] = arrayMap[pos.X, pos.Y];
                    }
                    return newPattern;

                default:
                    return newPattern;
            }
        }
    }
}
