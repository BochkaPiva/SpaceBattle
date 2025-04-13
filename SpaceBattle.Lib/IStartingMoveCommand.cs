namespace SpaceBattle.Lib;
public interface IStartingMoveCommand
{
    UObject objToMove
    {
        get;
    }
    IDictionary<string, object> order
    {
        get;
    }
}
