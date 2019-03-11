using Hangfire;
using Hangfire.Server;
using log4net;
using PriceWatcher.Core.Models;
using PriceWatcher.Core.Tools;
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
                ScrapingBrowser browser = new ScrapingBrowser();
                browser.AutoDetectCharsetEncoding = true;
                browser.AllowAutoRedirect = true;

                UserSettings.Current.WatchersSettings.ForEach(async settings =>
                {
                    if (settings.Enabled)
                    {
                        _context.WriteLog($"Check price for '{settings.Name}' at '{settings.Url}'");
                        var res = await browser.NavigateToPageAsync(new Uri(settings.Url));
                        var sele = res.Html.CssSelect(settings.CssSelector);
                        var priceRead = sele.FirstOrDefault();
                        if (priceRead != null && priceRead.Attributes.Contains(settings.DataAttributes))
                        {
                            _context.WriteLog($"Selector found for '{settings.Url}'");
                            double valuePrice = 0;
                            double.TryParse(priceRead.Attributes[settings.DataAttributes].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out valuePrice);
                            _context.WriteLog($"Price Found : {valuePrice} < {settings.LastPrice}");
                            if (valuePrice < settings.LastPrice)
                            {
                                _context.WriteLog($"New price detected '{valuePrice}' sending mail");
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
                _context.WriteError(ex);
            }
        }
    }
}
