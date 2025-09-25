using Microsoft.AspNetCore.SignalR.Client;

namespace WPFClient
{
    internal class WebSocketClient
    {
        private HubConnection? _connection;

        public async Task StartAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/chat")
                .WithAutomaticReconnect()
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Console.WriteLine($"{user}: {message}");
            });

            await _connection.StartAsync();
            Console.WriteLine("Connecting to the server...");
        }

        public async Task SendMessageAsync(string user, string message)
        {
            if (_connection == null)
            {
                return;
            }

            if (_connection.State == HubConnectionState.Connected)
            {
                await _connection.InvokeAsync("SendMessage", user, message);
            }
        }
    }
}
