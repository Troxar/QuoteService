using System;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Input;
using QuoteService.Service;

namespace QuoteService.WpfClient
{
    public partial class MainWindow : Window
    {
        private readonly ServiceController serviceController;

        public MainWindow()
        {
            InitializeComponent();

            string serviceName = "RandomQuoteService";
            serviceController = new ServiceController(serviceName);

            RefreshServiceButtons();
        }

        private void GetQuote_Click(object sender, RoutedEventArgs e)
        {
            const int bufferSize = 1024;

            Cursor currentCursor = Cursor;
            Cursor = Cursors.Wait;

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
                if (received <= 0)
                {
                    return;
                }

                QuoteTextBox.Text = Encoding.Unicode.GetString(buffer).Trim('\0');
            }
            catch (SocketException ex)
            {
                QuoteTextBox.Text = $"Quote getting error: {ex.Message}";
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

            Cursor = currentCursor;
        }

        private void StartServiceButton_Click(object sender, RoutedEventArgs e)
        {
            OnServiceCommand(sender, e);
        }

        private void StopServiceButton_Click(object sender, RoutedEventArgs e)
        {
            OnServiceCommand(sender, e);
        }

        private void RefreshQuotesButton_Click(object sender, RoutedEventArgs e)
        {
            OnServiceCommand(sender, e);
        }

        private void OnServiceCommand(object sender, RoutedEventArgs e)
        {
            Cursor currentCursor = Cursor;

            try
            {
                if (sender == StartServiceButton)
                {
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                }
                else if (sender == StopServiceButton)
                {
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                }
                else if (sender == RefreshQuotesButton)
                {
                    serviceController.ExecuteCommand(RandomQuoteService.commandRefresh);
                }
            }
            catch (System.ServiceProcess.TimeoutException ex)
            {
                QuoteTextBox.Text = $"Timeout: {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                QuoteTextBox.Text = string.Format("Error: {0} {1}", ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty);
            }
            finally
            {
                Cursor = currentCursor;
            }

            RefreshServiceButtons();
        }

        private void RefreshServiceButtons()
        {
            ServiceControllerStatus status = serviceController.Status;

            StartServiceButton.IsEnabled = status != ServiceControllerStatus.Running;
            StopServiceButton.IsEnabled = status != ServiceControllerStatus.Stopped;
            RefreshQuotesButton.IsEnabled = status == ServiceControllerStatus.Running;
        }
    }
}
