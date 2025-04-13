using Hwdtech;


namespace SpaceBattle.Lib;
using System.Collections.Concurrent;

public class SenderAdapter : ISender
{
    BlockingCollection<ICommand> queue;

    public SenderAdapter(BlockingCollection<ICommand> queue) => this.queue = queue;

    public void Send(object message)
    {
        queue.Add(IoC.Resolve<ICommand>("Commands.InterpretMessage", message));
    }
}
