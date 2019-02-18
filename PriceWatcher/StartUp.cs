using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.Owin;
using Owin;
using System;

[assembly: OwinStartup(typeof(PriceWatcher.Startup))]
namespace PriceWatcher
{
    public class Startup
    {
        /// <summary>
        /// The path match
        /// </summary>
        private const string PathMatch = "/hangfire";

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseColouredConsoleLogProvider();
            GlobalConfiguration.Configuration.UseConsole();
            GlobalConfiguration.Configuration.UseMemoryStorage();
            var dashboardOption = new DashboardOptions
            {
                Authorization = new[]
                {
                        new LocalRequestsOnlyAuthorizationFilter()
                    }
            };
            app.UseHangfireDashboard(PathMatch, dashboardOption);

            var options = new BackgroundJobServerOptions
            {
                Queues = new[] { "critical", "high", "default", "low", "idle" },
                WorkerCount = 1,
                HeartbeatInterval = TimeSpan.FromMinutes(1),
                ServerCheckInterval = TimeSpan.FromMinutes(1),
                SchedulePollingInterval = TimeSpan.FromMinutes(1)
            };

            app.UseHangfireServer(options);
        }
    }
}
