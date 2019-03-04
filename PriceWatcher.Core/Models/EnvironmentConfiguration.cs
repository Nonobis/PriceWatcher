using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Models
{
    public class EnvironmentConfiguration
    {
        public string MAIL_HOSTNAME { get; set; }

        public string MAIL_LOGIN { get; set; }

        public string MAIL_PASSWORD { get; set; }

        public int MAIL_PORT { get; set; }

        public bool MAIL_ENABLE_SSL { get; set; }

        public string MAIL_FROM { get; set; }

        public string MAIL_TARGETS { get; set; }

        public string EXCEPTIONLESS_URL { get; set; }

        public bool EXCEPTIONLESS_ENABLED { get; set; }

        public string EXCEPTIONLESS_APIKey { get; set; }
    }
}
