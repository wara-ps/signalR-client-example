namespace SignalrClient.Services
{
    public class SignalRService : ISignalRService
    {
        public event Action<string> OnLogin;
        public event Action<int> OnSelectBoard;
        public event Action<string, string, string> OnTryMoveAgent;

        public Task Login(string Id)
        {
            OnLogin?.Invoke(Id);
            return Task.CompletedTask;
        }

        public Task SelectBoard(int Id)
        {
            OnSelectBoard?.Invoke(Id);
            return Task.CompletedTask;
        }

        public Task TryMoveAgent(string agentId, string X, string Y)
        {
            OnTryMoveAgent?.Invoke(agentId, X, Y);
            return Task.CompletedTask;
        }
    }
}
