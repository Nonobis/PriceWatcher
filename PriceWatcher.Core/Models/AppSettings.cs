using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PriceWatcher.Core.Models;
using SimpleHelper.Core;
using System.Collections.Generic;
using System.IO;

namespace PriceWatcher.Core.Models
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
        /// Gets or sets the exceptionless settings.
        /// </summary>
        /// <value>The exceptionless settings.</value>
        public ExceptionlessSettings ExceptionlessSettings { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettings" /> class.
        /// </summary>
        public AppSettings()
        {

        }

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public static AppSettings Current { get; set; }


        /// <summary>
        /// Gets or sets the hosting environment.
        /// </summary>
        /// <value>The hosting environment.</value>
        [JsonIgnore]
        public IHostingEnvironment HostingEnvironment { get; set; }
    }
}
