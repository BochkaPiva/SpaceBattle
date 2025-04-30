namespace SpaceBattle.Lib;

public interface IRoutingService
{
    void RegisterRouter(int threadId, IRouter router);
    void UnregisterRouter(int threadId);
    void RouteMessage(IMessage message);
    void RegisterGame(int gameId, int threadId);
    void UnregisterGame(int gameId);
    int GetThreadForGame(int gameId);
} 