namespace SpaceBattle.Lib;
public class ServerThread
{
    public Thread thread { get; private set; }
    public ReceiverAdapter queue { get; private set; }
    bool stop = false;
    Action strategy;
    Action finishingStrategy;
    public ServerThread(ReceiverAdapter queue)
    {
        this.queue = queue;
        strategy = () =>
        {
            _handleCommand();
        };

        finishingStrategy = new Action(() =>
        {

        });
    
        thread = new Thread(() =>
        {
            while (!stop)
            {
                strategy();
            }
        });
    }
    internal void _stop()
    {
        finishingStrategy();
        stop = true;
    }
    internal void _handleCommand()
    {
        queue.Receive().Execute();
    }
    internal void _updateBehaviour(Action newBehaviour)
    {
        strategy = newBehaviour;
    }
    internal void _updateFinishingBehaviour(Action newBehaviour)
    {
        finishingStrategy = newBehaviour;
    }
    public void Start()
    {
        thread.Start();
    }
}
