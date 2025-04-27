namespace SpaceBattle.Lib;

public interface IRouter
{
    int ThreadId { get; }
    void Route(IMessage message);
    void RegisterGame(IGame game);
    void UnregisterGame(IGame game);
} 