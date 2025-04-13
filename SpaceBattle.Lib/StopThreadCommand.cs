namespace SpaceBattle.Lib;
public class StopThreadCommand : ICommand
{
    ServerThread stoppingThread;
    public StopThreadCommand(ServerThread stoppingThread) => this.stoppingThread = stoppingThread;

    public void Execute()
    {
        stoppingThread._stop();
    }
}
