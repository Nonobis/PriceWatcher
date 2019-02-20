using Newtonsoft.Json;
using SimpleHelper.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Models
{
    public class UserSettings
    {

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        [JsonIgnore]
        public static UserSettings Current { get; set; }

        /// <summary>
        /// Gets or sets the watchers settings.
        /// </summary>
        /// <value>The watchers settings.</value>
        public List<WatcherSettings> WatchersSettings { get; set; }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public static bool Load()
        {
            bool result = false;
            var _file = Path.Combine(AssemblyHelper.AssemblyDirectory, $"UserSettings.json");
            var jsonData = string.Empty;
            if (File.Exists(_file))
            {
                jsonData = File.ReadAllText(_file);

                // Désérialisation du Json
                Current = JsonConvert.DeserializeObject<UserSettings>(jsonData);
                if (Current != null)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Saves the specified override file. (Static Settings)
        /// </summary>
        /// <param name="overrideFile">if set to <c>true</c> [override file].</param>
        public static void Save(bool overrideFile)
        {
            if (Current == null)
            {
                return;
            }

            var _file = Path.Combine(AssemblyHelper.AssemblyDirectory, $"UserSettings.json");
            if (overrideFile && File.Exists(_file))
                File.Delete(_file);

            File.WriteAllText(_file, JsonConvert.SerializeObject(Current));
        }
    }
}
