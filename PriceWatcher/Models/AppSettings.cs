using Newtonsoft.Json;
using SimpleHelper;
using System.Collections.Generic;
using System.IO;

namespace PriceWatcher.Models
{
    public class AppSettings
    {
        /// <summary>
        /// Gets or sets the mail settings.
        /// </summary>
        /// <value>The mail settings.</value>
        public MailSettings MailSettings { get; set; }

        /// <summary>
        /// Gets or sets the watchers settings.
        /// </summary>
        /// <value>The watchers settings.</value>
        public List<WatcherSettings> WatchersSettings { get; set; }

        /// <summary>
        /// The settings
        /// </summary>
        public static AppSettings Settings;

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public static bool Load()
        {
            bool result = false;
            var _file = Path.Combine(AssemblyHelper.AssemblyDirectory, $"AppSettings.json");
            var jsonData = string.Empty;
            if (File.Exists(_file))
            {
                jsonData = File.ReadAllText(_file);

                // Désérialisation du Json
                Settings = JsonConvert.DeserializeObject<AppSettings>(jsonData);
                if (Settings != null)
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
            if (Settings == null)
            {
                return;
            }

            var _file = Path.Combine(AssemblyHelper.AssemblyDirectory, $"AppSettings.json");
            if (overrideFile && File.Exists(_file))
                File.Delete(_file);

            File.WriteAllText(_file, JsonConvert.SerializeObject(Settings));
        }
    }
}
