using boargame.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using SignalrClient.Services;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json;

namespace boardgame.Drivers
{
    public class SignalRDriver : DriverBase
    {
        private HubConnection _connection;
        private readonly ISignalRService _signalrService;

        public SignalRDriver(ILogger<SignalRDriver> logger, ISignalRService signalRService) : base(logger)
        {
         _signalrService = signalRService;

            _signalrService.OnLogin += onLogin;
            _signalrService.OnSelectBoard += onSelectBoard;
            _signalrService.OnTryMoveAgent += OnTryMoveAgent;
        }

        private void OnTryMoveAgent(string agent_id, string X, string Y)
        {

            var _tile = new
            {
                X = X,
                Y = Y,
                Type = 0,
                Excluded = false
            };
            string tile = JsonSerializer.Serialize(_tile).ToString();
            _connection.InvokeAsync("TryMoveAgent", agent_id, tile);

        }

        private void onSelectBoard(int id)
        {
            _connection.InvokeAsync("SelectBoard", id);
        }

        private void onLogin(string id)
        {
            _connection.InvokeAsync("Login", id);

        }

        public async Task InvokeHubMethodAsync(string methodName, string param1)
        {
            try
            {
                // Ensure the connection is started before invoking any hub methods.
                if (_connection.State == HubConnectionState.Connected)
                {
                    // Replace "MethodName" with the actual name of the hub method you want to call.
                    // You can pass parameters to the method if it expects any.
                    await _connection.InvokeAsync(methodName, param1);
                }
                else
                {
                    // Handle the case where the connection is not yet established.
                    // You might want to retry or display an error message.
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the invocation.
                // This could include network issues or server-side exceptions.
                Console.WriteLine($"Error invoking hub method: {ex.Message}");
            }
        }

        protected override async Task StartupDriverAsync(CancellationToken token)
        {
            //Hub Connection URL
            string url = "http://localhost:5188/gamehub"; //local session
            //string url = "https://bgbackend.waraps.org/gamehub"; //live session

            await Task.Delay(5000, token);
            _connection = new HubConnectionBuilder()
           .WithUrl(url)
           .Build();

            await _connection.StartAsync();


            //Add listeners
            _connection.On<string>("OnGameStateUpdate", (message) =>
            {
                // Deserialize the JSON message with indentation for readability
                string formattedJson = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(message), new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                // Print the formatted JSON string to the console
                Console.WriteLine($"GameStateUpdated:\n{formattedJson}");
            });
            _connection.On<string>("OnGetBoards", (message) =>
            {
                // Handle the received message here
                Console.WriteLine($"OnGetBoards: {message}");
            });

            //Login to create session and select board. (Invoke GetBoards to view available boards)
            //_connection.InvokeAsync("Login", "123");
            ////_connection.InvokeAsync("GetBoards");
            //_connection.InvokeAsync("SelectBoard", 1);

            // TODO: Arguments for methods
            int tile_x_coord = 1;
            int tile_y_coord = 2;
            string agentId_1 = "1001";
            string agentId_2 = "1002";
            string victimId = ""; //Check id in gamestateupdate

            var _tile = new
            {
                X = tile_x_coord,
                Y = tile_y_coord,
                Type = 0,
                Excluded = false
            };
            string tile = JsonSerializer.Serialize(_tile).ToString();

            //TODO: Uncomment to invoke method
            //_connection.InvokeAsync("TryMoveAgent", agentId_1, tile);
            //_connection.InvokeAsync("TrySearchArea", agentId_1, tile);
            //_connection.InvokeAsync("TryChargeBattery", agentId_1, agentId_2); // TryChargeBattery(agentCharging, agentToCharge)
            //_connection.InvokeAsync("TryPickupAgent", agentId_1, agentId_2); //TryPickupAgent(agentPickingup, agentToPickup)
            //_connection.InvokeAsync("TryPickupAid", "1001", agentId_1, agentId_2); //TryPickupAid(agentPickingup, agentToPickupFrom)
            //_connection.InvokeAsync("TryDeliverAid", "1001", agentId_1, victimId); //TryDeliverAid(agentId, victimId)
            //_connection.InvokeAsync("TrySaveHuman", "1001", agentId_1, victimId); //TrySaveHuman(agentId, victimId)
            //_connection.InvokeAsync("EndTurn");
            //_connection.InvokeAsync("LetAIDoRestOfTurn");
        }

        protected override async Task ExecuteDriverAsync(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(-1), token);
        }

        protected override Task ShutdownDriverAsync(CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
    public class Message
    {
        private string _content = "";
        public string Content { get => _content; set => _content = value; }
    }
}
