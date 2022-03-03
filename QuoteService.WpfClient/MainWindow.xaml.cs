using System.ServiceProcess;
using System.Windows;
using System.Windows.Input;

namespace QuoteService.WpfClient
{
    public partial class MainWindow : Window
    {
        private readonly ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new ViewModel();
            RefreshServiceButtons();
        }

        private void GetQuote_Click(object sender, RoutedEventArgs e)
        {
            Cursor currentCursor = Cursor;
            Cursor = Cursors.Wait;

            QuoteTextBox.Text = ViewModel.GetQuote();

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
            string errorMessage = string.Empty;

            if (sender == StartServiceButton)
            {
                viewModel.Start(out errorMessage);
            }
            else if (sender == StopServiceButton)
            {
                viewModel.Stop(out errorMessage);
            }
            else if (sender == RefreshQuotesButton)
            {
                viewModel.RefreshQuotes(out errorMessage);
            };

            if (!string.IsNullOrEmpty(errorMessage))
            {
                QuoteTextBox.Text = $"Timeout: {errorMessage}";
            }

            Cursor = currentCursor;

            RefreshServiceButtons();
        }

        private void RefreshServiceButtons()
        {
            ServiceControllerStatus status = viewModel.GetServiceStatus();

            StartServiceButton.IsEnabled = status != ServiceControllerStatus.Running;
            StopServiceButton.IsEnabled = status != ServiceControllerStatus.Stopped;
            RefreshQuotesButton.IsEnabled = status == ServiceControllerStatus.Running;
        }
    }
}
