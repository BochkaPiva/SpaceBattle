using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class Router : IRouter
{
    private readonly int threadId;
    private readonly IRoutingService routingService;
    private readonly ConcurrentDictionary<int, IGame> games = new();

    public Router(IRoutingService routingService, int threadId)
    {
        this.routingService = routingService;
        this.threadId = threadId;
    }

    public int ThreadId => threadId;

    public void Route(IMessage message)
    {
        if (games.TryGetValue(message.GameId, out var game))
        {
            game.ProcessMessage(message);
        }
        else
        {
            var targetThread = routingService.GetThreadForGame(message.GameId);
            if (targetThread != threadId)
            {
                routingService.RouteMessage(message);
            }
            else
            {
                throw new InvalidOperationException($"Game {message.GameId} not found in thread {threadId}");
            }
        }
    }

    public void RegisterGame(IGame game)
    {
        routingService.RegisterGame(game.Id, threadId);
        if (!games.TryAdd(game.Id, game))
        {
            routingService.UnregisterGame(game.Id);
            throw new InvalidOperationException($"Game {game.Id} already registered in thread {threadId}");
        }
    }

    public void UnregisterGame(IGame game)
    {
        if (games.TryRemove(game.Id, out _))
        {
            routingService.UnregisterGame(game.Id);
        }
    }
} 