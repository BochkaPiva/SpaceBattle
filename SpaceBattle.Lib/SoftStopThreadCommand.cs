using Hwdtech;


namespace SpaceBattle.Lib;
public class SoftStopThreadCommand : ICommand
{
    ServerThread stoppingThread;
    Action finishingTask;
    public SoftStopThreadCommand(ServerThread stoppingThread, Action finishingTask)
    {
        this.stoppingThread = stoppingThread;
        this.finishingTask = finishingTask;
    }
    
    public void Execute()
    {
        Action softStopStrategy = () => 
        {
            if (stoppingThread.queue.isEmpty())
            {
                int threadId = IoC.Resolve<int>("Threading.GetThreadId", this.stoppingThread);
                ICommand hardStopThread = IoC.Resolve<ICommand>("Threading.HardStop", threadId, this.finishingTask);
                IoC.Resolve<ICommand>("Threading.SendCommand", threadId, hardStopThread).Execute();
            }
            else
            {
                stoppingThread._handleCommand();
            }
        };
        new UpdateBehaviourCommand(stoppingThread, softStopStrategy).Execute();
    }
}
