using SimpleHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Logging;

namespace PriceWatcher
{
    internal static class ConfigureService
    {
        /// <summary>
        /// Configures this instance.
        /// </summary>
        internal static void Configure()
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceHosted>(s =>
                {
                    s.ConstructUsing(name => new ServiceHosted());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                    s.WhenPaused(tc => { });
                    s.WhenContinued(tc => { });
                });
                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1); // restart the service after 1 minute
                    rc.SetResetPeriod(1);
                });
                x.EnableSessionChanged();
                x.RunAsLocalSystem();
                x.SetDescription(AssemblyHelper.AssemblyDescription);
                x.SetDisplayName(AssemblyHelper.AssemblyTitle);
                x.SetServiceName(AssemblyHelper.AssemblyTitle);
                x.RunAsLocalSystem();
                x.UseLog4Net();
                x.UseAssemblyInfoForServiceInfo();
                x.StartAutomatically();
            });
        }
    }
}
