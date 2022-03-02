using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace QuoteService.Server
{
    public class QuoteServer
    {
        TcpListener listener;
        int port;
        string filename;
        List<string> quotes;
        Random random;
        Thread listenerThread;

        public QuoteServer() : this("quotes.txt")
        {

        }

        public QuoteServer(string filename) : this(filename, 7890)
        {

        }

        public QuoteServer(string filename, int port)
        {
            this.filename = filename;
            this.port = port;

            random = new Random(Guid.NewGuid().GetHashCode());
        }

        public void ReadQuotes()
        {
            if (File.Exists(filename))
            {
                quotes = File.ReadAllLines(filename).ToList();
            }
        }

        public string GetRandomQuote()
        {
            if (quotes == null || quotes.Count == 0)
            {
                return "No quotes";
            }

            int index = random.Next(0, quotes.Count);
            return quotes[index];
        }

        public void Start()
        {
            ReadQuotes();

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

        void ListenerThread()
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

                    string message = GetRandomQuote();
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
            ReadQuotes();
        }
    }
}