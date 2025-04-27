namespace SpaceBattle.Lib;

public interface IGame
{
    int Id { get; }
    void ProcessMessage(IMessage message);
    bool HasProcessedMessage(IMessage message);
} 