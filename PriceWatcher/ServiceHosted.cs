using Hangfire;
using Microsoft.Owin.Hosting;
using PriceWatcher.Jobs;
using PriceWatcher.Models;
using SimpleHelper;
using System;
using Topshelf.Squirrel.Updater.Interfaces;

namespace PriceWatcher
{
    /// <summary>
    /// Class ServiceHosted.
    /// </summary>
    public class ServiceHosted : ISelfUpdatableService
    {
        /// <summary>
        /// HangFire EndPoint
        /// </summary>
        private const string HangFireEndpoint = "http://*:12346";

        // Background Job Server
        /// <summary>
        /// The bg server
        /// </summary>
        private BackgroundJobServer _bgServer = null;

        /// <summary>
        /// HangFire Host
        /// </summary>
        private IDisposable _host;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHosted" /> class.
        /// </summary>
        public ServiceHosted()
        {
        }

        /// <summary>
        /// Stops the specified host control.
        /// </summary>
        public void Stop()
        {
            _bgServer?.Dispose();
            _host?.Dispose();
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            // HangFire Server
            _host = WebApp.Start<Startup>(HangFireEndpoint);
            Console.WriteLine($"HangFire : {HangFireEndpoint}/hangfire");

            // Loading AppSettings
            AppSettings.Load();

            //you have to create an instance of background job server at least once for background jobs to run
            _bgServer = new BackgroundJobServer();

            // Run once
            var sTitle = AssemblyHelper.AssemblyTitle;
            BackgroundJob.Enqueue(() => Console.WriteLine($"{sTitle} is now started !"));

            //Start Recurring Job
            BackgroundJob.Enqueue<CheckPriceJobs>(p => p.Execute(null));
            //RecurringJob.AddOrUpdate<CheckPriceJobs>(p => p.Execute(null), "0 6 * * *"); // Daily at 6 AM
        }

    }
}
