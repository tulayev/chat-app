using System.Windows;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WebSocketClient _client = new();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += async (s, e) =>
            {
                await _client.StartAsync();
                await _client.SendMessageAsync("Umarbek", $"Hello from WPF_{new Random().Next(1, 999)}");
            };
        }
    }
}