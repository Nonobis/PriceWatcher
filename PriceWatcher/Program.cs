using log4net.Config;
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
    class Program
    {
        private static readonly LogWriter Log = HostLogger.Get(typeof(Program));

        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            try
            {
                Log.InfoFormat("##########   Starting service '{0}', V '{1}'   ##########",
                                AssemblyHelper.AssemblyTitle,
                                AssemblyHelper.AssemblyVersion);

                // Add the event handler for handling unhandled  exceptions to the event.
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                ConfigureService.Configure();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            finally
            {
                Log.InfoFormat("##########   Stoppping service '{0}', V '{1}'   ##########",
                                AssemblyHelper.AssemblyTitle,
                                AssemblyHelper.AssemblyVersion);
            }
        }

        /// <summary>
        /// Currents the domain_ unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var report = (Exception)e.ExceptionObject;
            if (report != null)
            {
                Log.Error(report);
            }
        }
    }
}
