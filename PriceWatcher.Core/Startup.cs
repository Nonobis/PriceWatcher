using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hangfire;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.Encodings.Web;
using Hangfire.MemoryStorage;
using SimpleHelper.Core;
using PriceWatcher.Core.Models;
using PriceWatcher.Jobs;
using PriceWatcher.Core.Filters;
using log4net;
using Exceptionless;
using SimpleExtension.Core;
using PriceWatcher.Core.Tools;

namespace PriceWatcher.Core
{
    public class Startup
    {

        /// <summary>
        /// Logger Log4Net
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(Startup));

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="env">The env.</param>
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            this.HostingEnvironment = env;
            this.Configuration = configuration;
            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <value>The hosting environment.</value>
        /// <param name="env">The env.</param>
        public IHostingEnvironment HostingEnvironment { get; private set; }

        /// <summary>
        /// Gets or sets the bg server.
        /// </summary>
        /// <value>The bg server.</value>
        private BackgroundJobServer _bgServer { get; set; }

        /// <summary>
        /// Configures the specified application.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Configure the error handler to show an error page.
                app.UseExceptionHandler(errorApp =>
                {
                    // Normally you'd use MVC or similar to render a nice page.
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "text/html";
                        await context.Response.WriteAsync("<html><body>\r\n");
                        await context.Response.WriteAsync("We're sorry, we encountered an un-expected issue with your application.<br>\r\n");

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            // This error would not normally be exposed to the client
                            await context.Response.WriteAsync("<br>Error: " + HtmlEncoder.Default.Encode(error.Error.Message) + "<br>\r\n");
                        }
                        await context.Response.WriteAsync("<br><a href=\"/\">Home</a><br>\r\n");
                        await context.Response.WriteAsync("</body></html>\r\n");
                        await context.Response.WriteAsync(new string(' ', 512)); // Padding for IE
                    });
                });

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            ConfigureExceptionless();
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            loggerFactory.AddLog4Net();

            // Hangifre
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new NoAuthFilter() }
            });

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { "critical", "high", "default", "low", "idle" },
                WorkerCount = 1,
                HeartbeatInterval = new TimeSpan(0, 1, 0),
                ServerCheckInterval = new TimeSpan(0, 1, 0),
                SchedulePollingInterval = new TimeSpan(0, 1, 0)
            });

            app.UseAuthentication();

            var option = new RewriteOptions();
            option.AddRedirect("^$", "hangfire");
            app.UseRewriter(option);
            app.UseCors("AllowAll");
            SetUpHangfireJobs();
        }


        /// <summary>
        /// Configures the exceptionless.
        /// </summary>
        private void ConfigureExceptionless()
        {
            ExceptionlessClient.Default.Configuration.ServerUrl = AppSettings.Current.ExceptionlessSettings.ServerUrl;
            ExceptionlessClient.Default.Configuration.IncludePrivateInformation = false;
            ExceptionlessClient.Default.Configuration.Enabled = AppSettings.Current.ExceptionlessSettings.Enabled;
            ExceptionlessClient.Default.Configuration.UpdateSettingsWhenIdleInterval = TimeSpan.FromMinutes(1);
            ExceptionlessClient.Default.Configuration.QueueMaxAge = TimeSpan.FromDays(7);
            ExceptionlessClient.Default.Configuration.QueueMaxAttempts = 3;
            ExceptionlessClient.Default.Configuration.ApiKey = AppSettings.Current.ExceptionlessSettings.ApiKey;
            ExceptionlessClient.Default.Configuration.Settings.Changed += ExceptionlessSettingsClient_Changed;
            ExceptionlessClient.Default.Startup();
        }

        /// <summary>
        /// Exceptionlesses the settings client changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ExceptionlessSettingsClient_Changed(object sender, global::Exceptionless.Models.Collections.ChangedEventArgs<KeyValuePair<string, string>> e)
        {
            // Plus d'info : https://github.com/exceptionless/Exceptionless.Net/wiki/Client-Configuration-Values
            if (e != null)
                ExceptionlessClient.Default.CreateLog($"Received setting change, Item => Key : {e.Item.Key}, Value : {e.Item.Value}", Exceptionless.Logging.LogLevel.Debug);
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {

            //Adding Cors Config
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    p => p.AllowAnyOrigin().
                    AllowAnyMethod().
                    AllowAnyHeader().
                    AllowCredentials());
            });

            // Add framework services.
            services.AddMvc();

            services.AddOptions();
            AppSettings.Current = new AppSettings();
            services.ConfigurePOCO(Configuration.GetSection("AppSettings"), () => AppSettings.Current);
            AppSettings.Current.HostingEnvironment = HostingEnvironment;
            services.AddSingleton(Configuration);

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.Name = $"_{AssemblyHelper.AssemblyTitle}Cookie";
            });
            

            services.AddHangfire(x => x.UseStorage(new MemoryStorage()
            {
                
            }));
            
        }

        /// <summary>
        /// Sets up jobs.
        /// </summary>
        private void SetUpHangfireJobs()
        {
            //you have to create an instance of background job server at least once for background jobs to run
            _bgServer = new BackgroundJobServer();

            // Run once
            var sTitle = AssemblyHelper.AssemblyTitle;
            BackgroundJob.Enqueue(() => Console.WriteLine($"{sTitle} is now started !"));

            // Starting Recurruning Job
            RecurringJob.AddOrUpdate<CheckPriceJobs>(p => p.Execute(null), "*/45 * * * *"); // Every 45 Minutes
        }

    }
}
