using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoMWallet.Tool
{
    class Caching
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool TryReadFile<T>(string path, out T obj)
        {
            obj = default(T);

            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                var objDecrypted = Decrypt(File.ReadAllText(path));
                obj = JsonConvert.DeserializeObject<T>(objDecrypted);
                return true;
            }
            catch (Exception e)
            {
                _log.Error($"TryReadFile<{obj.GetType()}>: {e}");
                return false;
            }
        }

        public static void Persist<T>(string path, T obj)
        {
            string objEncrypted = Encrypt(JsonConvert.SerializeObject(obj));
            File.WriteAllText(path, objEncrypted);
        }

        //unused, allready encrypted maybee for later
        private static string Encrypt(string str)
        {
            return str;
        }

        //unused, allready encrypted maybee for later
        private static string Decrypt(string str)
        {
            return str;
        }
    }
}
