using Hwdtech;


namespace SpaceBattle.Lib;
public class HardStopThreadCommand : ICommand
{
    ServerThread stoppingThread;
    Action finishingTask;
    SenderAdapter sender;
    public HardStopThreadCommand(ServerThread stoppingThread, Action finishingTask, SenderAdapter sender)
    {
        this.stoppingThread = stoppingThread;
        this.finishingTask = finishingTask;
        this.sender = sender;
    }
    public void Execute()
    {
        UpdateFinishBehaviourCommand updateFinishingBehaviour = new UpdateFinishBehaviourCommand(this.stoppingThread, this.finishingTask);
        StopThreadCommand stopThread = new StopThreadCommand(this.stoppingThread);

        int threadId = IoC.Resolve<int>("Threading.GetThreadId", this.stoppingThread);

        updateFinishingBehaviour.Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", threadId, stopThread).Execute();
    }
}
