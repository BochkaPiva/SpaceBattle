using Hwdtech;


namespace SpaceBattle.Lib;

public class PushCommand : IStrategy 
{   
    public object Run(params object[] args)
    {
        string id = (string)args[0];
        ICommand command = (ICommand)args[1];

        var queue = IoC.Resolve<Queue<ICommand>>("GetQueue", id);

        return new ActionCommand(() => { queue.Enqueue(command); });
    }
}
