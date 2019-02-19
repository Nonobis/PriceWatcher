using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Models
{  
    /// <summary>
     /// Class ExceptionlessSettings.
     /// </summary>
    public class ExceptionlessSettings
    {
        /// <summary>
        /// Gets or sets the server URL.
        /// </summary>
        /// <value>The server URL.</value>
        public string ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>The API key.</value>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ExceptionlessSettings"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }
    }
}
