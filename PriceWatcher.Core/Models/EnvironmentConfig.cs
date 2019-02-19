using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Models
{
    public class EnvironmentConfig
    {
        public string HostName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public bool EnableSsl { get; set; }

        public string From { get; set; }


        public string Tagets { get; set; }
    }
}
