using Hwdtech;

namespace SpaceBattle.Lib;

public class SoftStopThreadCommand : ICommand
{
    private readonly ServerThread stoppingThread;
    private readonly Action finishingTask;

    public SoftStopThreadCommand(ServerThread stoppingThread, Action finishingTask)
    {
        this.stoppingThread = stoppingThread;
        this.finishingTask = finishingTask;
    }
    
    public void Execute()
    {
        stoppingThread._updateFinishingBehaviour(finishingTask);
        
        while (!stoppingThread.queue.isEmpty())
        {
            try
            {
                stoppingThread.queue.Receive();
            }
            catch { }
        }
        
        stoppingThread._stop();

        var startTime = DateTime.UtcNow;
        while (stoppingThread.IsRunning)
        {
            if ((DateTime.UtcNow - startTime).TotalSeconds > 3)
            {
                throw new TimeoutException("Thread did not stop within timeout");
            }
            Thread.Sleep(10);
        }
    }
}
