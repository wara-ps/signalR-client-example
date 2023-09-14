namespace SignalrClient.Services
{
    public interface ISignalRService
    {
        event Action<string> OnLogin;
        event Action<int> OnSelectBoard;
        event Action<string, string, string> OnTryMoveAgent;
        Task Login(string Id);
        Task SelectBoard(int Id);
        Task TryMoveAgent(string agentId, string X, string Y);
    }
}
