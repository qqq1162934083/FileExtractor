using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Libs
{
    public class ConfigUtils
    {
        public static T ReadJsonConfig<T>(string path)
        {
            if (!File.Exists(path)) return default(T);
            var configStr = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject<T>(configStr);
        }

        public static object ReadJsonConfig(string path, Type type)
        {
            if (!File.Exists(path)) return null;
            var configStr = File.ReadAllText(path, Encoding.UTF8);
            return JsonConvert.DeserializeObject(configStr, type);
        }

        public static void WriteJsonConfig(string path, object config, bool overwrite = true)
        {
            var dirInfo = Directory.GetParent(path);
            if (!dirInfo.Exists) dirInfo.Create();
            if (!File.Exists(path)) File.Create(path).Dispose();
            var configStr = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(path, configStr);
        }
    }
}
