namespace SpaceBattle.Lib;
public class UpdateBehaviourCommand : ICommand
{
    Action behaviour;
    ServerThread thread;

    public UpdateBehaviourCommand(ServerThread thread, Action newBehaviour)
    {
        this.behaviour = newBehaviour;
        this.thread = thread;
    }
    public void Execute()
    {
        thread._updateBehaviour(behaviour);
    }
}
