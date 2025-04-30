using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class RoutingService : IRoutingService
{
    private readonly ConcurrentDictionary<int, IRouter> routers = new();
    private readonly ConcurrentDictionary<int, int> gameToThread = new();

    public void RegisterRouter(int threadId, IRouter router)
    {
        if (!routers.TryAdd(threadId, router))
        {
            throw new InvalidOperationException($"Router already registered for thread {threadId}");
        }
    }

    public void UnregisterRouter(int threadId)
    {
        routers.TryRemove(threadId, out _);
    }

    public void RouteMessage(IMessage message)
    {
        if (gameToThread.TryGetValue(message.GameId, out var threadId) && routers.TryGetValue(threadId, out var router))
        {
            router.Route(message);
        }
        else
        {
            throw new InvalidOperationException($"Game {message.GameId} not found in any thread");
        }
    }

    public void RegisterGame(int gameId, int threadId)
    {
        if (!routers.ContainsKey(threadId))
        {
            throw new InvalidOperationException($"Thread {threadId} not found");
        }
        gameToThread.TryAdd(gameId, threadId);
    }

    public void UnregisterGame(int gameId)
    {
        gameToThread.TryRemove(gameId, out _);
    }

    public int GetThreadForGame(int gameId)
    {
        if (gameToThread.TryGetValue(gameId, out var threadId))
        {
            return threadId;
        }
        throw new InvalidOperationException($"Game {gameId} not found");
    }
} 