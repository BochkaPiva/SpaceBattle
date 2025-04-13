namespace SpaceBattle.Lib;

public class StartQueueStrategy : IStrategy 
{
    public object Run(params object[] args)
    {
        var queue = (Queue<ICommand>)args[0];

        return new StartQueue(queue);
    }
}
