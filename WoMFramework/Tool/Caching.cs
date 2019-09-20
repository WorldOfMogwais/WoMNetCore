namespace WoMFramework.Tool
{
    using log4net;
    using Newtonsoft.Json;
    using System;
    using System.Reflection;

    public class Caching
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static bool TryReadFile<T>(string path, out T obj)
        {
            obj = default(T);

            try
            {
                var objDecrypted = Decrypt(SystemInteraction.ReadAllText(path));
                obj = JsonConvert.DeserializeObject<T>(objDecrypted);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"TryReadFile<{obj?.GetType()}>: {e}");
                return false;
            }
        }

        public static void Persist<T>(string path, T obj)
        {
            var objEncrypted = Encrypt(JsonConvert.SerializeObject(obj));
            SystemInteraction.Persist(path, objEncrypted);
        }

        //unused, already encrypted maybe for later
        private static string Encrypt(string str)
        {
            return str;
        }

        //unused, already encrypted maybe for later
        private static string Decrypt(string str)
        {
            return str;
        }
    }
}
