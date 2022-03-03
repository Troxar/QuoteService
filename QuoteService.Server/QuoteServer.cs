using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuoteService.Server
{
    public class QuoteServer
    {
        private readonly IQuoteRepository quotes;
        private readonly int port;
        private TcpListener listener;
        private Thread listenerThread;
        
        public QuoteServer(IQuoteRepository quotes, int port)
        {
            this.quotes = quotes;
            this.port = port;
        }

        public void Start()
        {
            quotes.ReadQuotes();

            listenerThread = new Thread(ListenerThread);
            listenerThread.IsBackground = true;
            listenerThread.Name = "Listener";
            listenerThread.Start();
        }

        public void Stop()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        public void Suspend()
        {
            if (listener != null)
            {
                listener.Stop();
            }
        }

        public void Resume()
        {
            if (listener != null)
            {
                listener.Start();
            }
        }

        private void ListenerThread()
        {
            const string ipString = "127.0.0.1";
            IPAddress ipAddress = IPAddress.Parse(ipString);

            try
            {
                listener = new TcpListener(ipAddress, port);
                listener.Start();

                while (true)
                {
                    Socket clientSocket = listener.AcceptSocket();

                    string message = quotes.GetRandomQuote();
                    UnicodeEncoding encoder = new UnicodeEncoding();
                    byte[] buffer = encoder.GetBytes(message);

                    clientSocket.Send(buffer, buffer.Length, 0);
                    clientSocket.Close();
                }
            }
            catch (SocketException e)
            {
                Trace.TraceError(string.Format("QuoteServer {0}", e.Message));
            }
        }

        public void RefreshQuotes()
        {
            quotes.ReadQuotes();
        }
    }
}