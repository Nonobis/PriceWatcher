using Hangfire;
using Hangfire.Server;
using MailKit.Net.Smtp;
using MimeKit;
using PriceWatcher.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PriceWatcher.Jobs
{
    public class SendMailJob
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
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="readerId">The reader identifier.</param>
        /// <param name="needReturnOnGPO">if set to <c>true</c> [export final target].</param>
        [Queue("default")]
        [AutomaticRetry(Attempts = 5, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void Execute(PerformContext context, WatcherSettings watcher, double newPrice)
        {
            lock (Lock)
            {
                _context = context;
                DoWork(watcher, newPrice);
            };
        }

        private void DoWork(WatcherSettings watcher, double newPrice)
        {
            var message = new MimeMessage();
            AppSettings.Current.MailSettings.Targets.ForEach(currentMailTarget =>
            {
                if (!string.IsNullOrEmpty(currentMailTarget))
                    message.From.Add(new MailboxAddress(currentMailTarget));
            });

            message.Subject = $"Change of price on item [{watcher.Name}]";
            message.Body = new TextPart("plain")
            {
                Text = $@"Hello, 

Price of item '{watcher.Name}' just changed from '{watcher.LastPrice}' to '{newPrice}').
Url : {watcher.Url}

Bye."
            };

            using (var client = new SmtpClient())
            {
                // Accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                client.Connect(AppSettings.Current.MailSettings.HostName, AppSettings.Current.MailSettings.Port, AppSettings.Current.MailSettings.EnableSsl);
                client.Authenticate(AppSettings.Current.MailSettings.Login, AppSettings.Current.MailSettings.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
