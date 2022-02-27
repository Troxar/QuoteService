using System;
using System.IO;
using System.ServiceProcess;
using QuoteService.Server;

namespace QuoteService.Service
{
    public partial class RandomQuoteService : ServiceBase
    {
        QuoteServer quoteServer;

        public const int commandRefresh = 128;

        public RandomQuoteService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            string filename = Properties.Settings.Default.QuoteFileName;
            string filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            int port = Properties.Settings.Default.PortNumber;
            
            quoteServer = new QuoteServer(filepath, port);
            quoteServer.Start();
        }

        protected override void OnStop()
        {
            if (quoteServer != null)
            {
                quoteServer.Stop();
            }
        }

        protected override void OnCustomCommand(int command)
        {
            if (quoteServer == null)
            {
                return;
            }

            switch (command)
            {
                case commandRefresh:
                    quoteServer.RefreshQuotes();
                    break;

                default:
                    break;
            }
        }
    }
}
