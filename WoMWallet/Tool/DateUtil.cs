using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoMWallet.Tool
{
    public class DateUtil
    {
        public static DateTime GetBlockLocalDateTime(int blocktime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var currentTime = epoch.AddSeconds(blocktime);
            return currentTime.ToLocalTime();
        }
    }
}
