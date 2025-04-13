namespace SpaceBattle.Lib;
public class UpdateFinishBehaviourCommand : ICommand
{
    Action behaviour;
    ServerThread thread;

    public UpdateFinishBehaviourCommand(ServerThread thread, Action newBehaviour)
    {
        this.behaviour = newBehaviour;
        this.thread = thread;
    }
    public void Execute()
    {
        thread._updateFinishingBehaviour(behaviour);
    }
}
