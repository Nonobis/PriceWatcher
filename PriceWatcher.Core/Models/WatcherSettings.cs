using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Models
{
    /// <summary>
    /// Class WatcherSettings.
    /// </summary>
    public class WatcherSettings
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the CSS selector.
        /// </summary>
        /// <value>The CSS selector.</value>
        public string CssSelector { get; set; }

        public string DataAttributes { get; set; }

        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the last price.
        /// </summary>
        /// <value>The last price.</value>
        public double LastPrice { get; set; }
    }
}
