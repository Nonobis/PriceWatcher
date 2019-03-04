using Hangfire;
using Hangfire.Server;
using log4net;
using PriceWatcher.Core.Models;
using ScrapySharp.Extensions;
using ScrapySharp.Network;
using System;
using System.Globalization;
using System.Linq;

namespace PriceWatcher.Jobs
{
    public class CheckPriceJobs
    {
        /// <summary>
        /// The context
        /// </summary>
        static PerformContext _context;

        /// <summary>
        /// The lock
        /// </summary>
        static readonly object Lock = new object();

        /// <summary>
        /// Logger Log4Net
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(CheckPriceJobs));

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="readerId">The reader identifier.</param>
        /// <param name="needReturnOnGPO">if set to <c>true</c> [export final target].</param>
        [Queue("default")]
        [AutomaticRetry(Attempts = 5, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void Execute(PerformContext context)
        {
            lock (Lock)
            {
                _context = context;
                DoWork();
            };
        }

        private void DoWork()
        {
            try
            {
                UserSettings.Current.WatchersSettings.ForEach(async settings =>
                {
                    if (settings.Enabled)
                    {
                        ScrapingBrowser browser = new ScrapingBrowser();
                        browser.AutoDetectCharsetEncoding = true;
                        browser.AllowAutoRedirect = true;

                        var res = await browser.NavigateToPageAsync(new Uri(settings.Url));
                        var sele = res.Html.CssSelect(settings.CssSelector);
                        var priceRead = sele.FirstOrDefault();
                        if (priceRead != null && priceRead.Attributes.Contains(settings.DataAttributes))
                        {
                            Log.Info($"Selector found for '{settings.Url}'");
                            double valuePrice = 0;
                            double.TryParse(priceRead.Attributes[settings.DataAttributes].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out valuePrice);
                            Log.Info($"Price Found : {valuePrice} < {settings.LastPrice}");
                            if (valuePrice < settings.LastPrice)
                            {
                                Log.Info($"New price detected '{valuePrice}' sending mail");
                                BackgroundJob.Enqueue<SendMailJob>(p => p.Execute(null, settings, valuePrice));
                            }
                            settings.LastPrice = valuePrice;
                        }
                    }
                });
                UserSettings.Save(true);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
    }
}
