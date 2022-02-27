using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace QuoteService.WpfClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

                TextQuote.Text = Encoding.Unicode.GetString(buffer).Trim('\0');
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Quote getting error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
