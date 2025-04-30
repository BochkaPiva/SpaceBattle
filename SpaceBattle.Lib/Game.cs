using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class Game : IGame
{
    private readonly int id;
    private readonly ConcurrentQueue<IMessage> processedMessages = new();

    public Game(int id)
    {
        this.id = id;
    }

    public int Id => id;

    public void ProcessMessage(IMessage message)
    {
        processedMessages.Enqueue(message);
    }

    public bool HasProcessedMessage(IMessage message)
    {
        return processedMessages.ToArray().Contains(message);
    }
} 