namespace WoMWallet.Tool
{
    using System;

    public class DateUtil
    {
        public static DateTime GetBlockLocalDateTime(int blockTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime currentTime = epoch.AddSeconds(blockTime);
            return currentTime.ToLocalTime();
        }
    }
}
