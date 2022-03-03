using System;
using System.Net.Sockets;
using System.Text;
using System.ServiceProcess;
using QuoteService.Service;

namespace QuoteService.WpfClient
{
    internal class ViewModel : IDisposable
    {
        private ServiceController serviceController;
        private delegate void ServiceCommand();

        public ViewModel()
        {
            string serviceName = "RandomQuoteService";
            serviceController = new ServiceController(serviceName);
        }

        public ServiceControllerStatus GetServiceStatus()
        {
            return serviceController.Status;
        }

        public static string GetQuote()
        {
            const int bufferSize = 1024;

            string serverName = Properties.Settings.Default.ServerName;
            int port = Properties.Settings.Default.PortNumber;

            TcpClient client = new TcpClient();
            NetworkStream stream = null;

            try
            {
                client.Connect(serverName, port);
                stream = client.GetStream();

                byte[] buffer = new byte[bufferSize];
                int received = stream.Read(buffer, 0, bufferSize);

                return received <= 0 ? "" : Encoding.Unicode.GetString(buffer).Trim('\0');
            }
            catch (SocketException ex)
            {
                return $"Quote getting error: {ex.Message}";
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (client.Connected)
                {
                    client.Close();
                }
            }
        }

        private void ExecuteRefreshQuotesCommand()
        {
            serviceController.ExecuteCommand(RandomQuoteService.commandRefresh);
        }

        public void Start(out string errorMessage)
        {
            ExecuteServiceCommand(serviceController.Start, ServiceControllerStatus.Running, out errorMessage);
        }

        public void Stop(out string errorMessage)
        {
            ExecuteServiceCommand(serviceController.Stop, ServiceControllerStatus.Stopped, out errorMessage);
        }

        public void RefreshQuotes(out string errorMessage)
        {
            ExecuteServiceCommand(ExecuteRefreshQuotesCommand, null, out errorMessage);
        }

        private void ExecuteServiceCommand(ServiceCommand serviceCommand, ServiceControllerStatus? desiredStatus, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                serviceCommand();
                if (desiredStatus != null)
                {
                    serviceController.WaitForStatus((ServiceControllerStatus)desiredStatus, TimeSpan.FromSeconds(10));
                }
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                errorMessage = $"Timeout: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                errorMessage = string.Format("Error: {0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
            }
        }

        public void Dispose()
        {
            if (serviceController != null)
            {
                serviceController.Dispose();
                serviceController = null;
            }
        }
    }
}
