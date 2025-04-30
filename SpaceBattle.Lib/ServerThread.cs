namespace SpaceBattle.Lib;

public class ServerThread
{
    public Thread thread { get; private set; }
    public ReceiverAdapter queue { get; private set; }
    private volatile bool stop = false;
    private Action strategy;
    private Action finishingStrategy;
    private readonly object lockObject = new object();

    public bool IsRunning => !stop;

    public ServerThread(ReceiverAdapter queue)
    {
        this.queue = queue;
        strategy = () => _handleCommand();
        finishingStrategy = () => { };
    
        thread = new Thread(() =>
        {
            try
            {
                while (!stop)
                {
                    strategy();
                }
            }
            finally
            {
                lock (lockObject)
                {
                    finishingStrategy();
                }
            }
        });
    }

    internal void _stop()
    {
        lock (lockObject)
        {
            stop = true;
            finishingStrategy();
        }
    }

    internal void _handleCommand()
    {
        try
        {
            queue.Receive().Execute();
        }
        catch (OperationCanceledException)
        {
            while (!queue.isEmpty())
            {
                try
                {
                    queue.Receive();
                }
                catch { }
            }
            _stop();
        }
    }

    internal void _updateBehaviour(Action newBehaviour)
    {
        lock (lockObject)
        {
            strategy = newBehaviour;
        }
    }

    internal void _updateFinishingBehaviour(Action newBehaviour)
    {
        lock (lockObject)
        {
            finishingStrategy = newBehaviour;
        }
    }

    public void Start()
    {
        thread.Start();
    }
}
