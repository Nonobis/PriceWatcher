using SimpleHelper;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;
using Topshelf.Squirrel.Updater;
using Topshelf.Squirrel.Updater.Interfaces;

namespace PriceWatcher
{
    internal static class ConfigureService
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        internal static void Configure()
        {
            // Start Service Updater
            IUpdater selfupdater = null;
            ServiceHosted service = new ServiceHosted();

            try
            {
                IUpdateManager updateManager = new UpdateManager("", AssemblyHelper.AssemblyTitle);
                selfupdater = new RepeatedTimeUpdater(updateManager).SetCheckUpdatePeriod(TimeSpan.FromMinutes(30));
                selfupdater.Start();
            }
            catch (Exception exx)
            {
                Console.WriteLine("'{0}' is not installed via Squirrel. Install program first.", AssemblyHelper.AssemblyTitle);
            }

            // Start TopShelf 
            var x = new SquirreledHost(service, AssemblyHelper.CurrentAssembly, selfupdater, true, RunAS.LocalSystem);
            x.ConfigureAndRun(HostConfig =>
            {
                HostConfig.Service<ServiceHosted>(s =>
                {
                    s.ConstructUsing(name => new ServiceHosted());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => { });
                    s.WhenContinued(tc => { });
                });
                HostConfig.EnableServiceRecovery(rc => rc.RestartService(1));
                HostConfig.EnableSessionChanged();
                HostConfig.UseLog4Net();
                HostConfig.StartAutomatically();
            });
        }
    }
}
