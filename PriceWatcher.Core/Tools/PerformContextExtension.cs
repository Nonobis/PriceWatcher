using Exceptionless;
using Hangfire.Console;
using Hangfire.Server;
using SimpleExtension.Core;
using SimpleHelper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceWatcher.Core.Tools
{
    public static class PerformContextExtension
    {
        /// <summary>
        /// Writes the error.
        /// </summary>
        /// <param name="pContext">The p context.</param>
        /// <param name="pException">The p exception.</param>
        public static void WriteError(this PerformContext pContext, Exception pException)
        {
            pException.ToExceptionless().Submit();
            if (pException?.InnerException != null)
                pContext.WriteLine(pException.FormatForHuman());
            pException.ToExceptionless().SetMessage($"Job '{pContext.BackgroundJob.Job.Type}' n°{pContext.BackgroundJob.Id} : {pException}").MarkAsCritical().SetVersion(AssemblyHelper.AssemblyVersion);
        }

        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="pContext">The p context.</param>
        /// <param name="pMessage">The p message.</param>
        public static void WriteLog(this PerformContext pContext, string pMessage)
        {
            pContext.WriteLine(pMessage);
        }
    }
}
