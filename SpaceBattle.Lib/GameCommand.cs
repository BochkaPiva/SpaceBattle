using Hwdtech;


namespace SpaceBattle.Lib;


public class GameCommand : SpaceBattle.Lib.ICommand
{
    private Queue<SpaceBattle.Lib.ICommand> queue;

    private object scope;

    public GameCommand(object scope, Queue<SpaceBattle.Lib.ICommand> queue)
    {
        this.scope = scope;
        this.queue = queue;
    }

    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<SpaceBattle.Lib.ICommand>("StartQueue", queue).Execute();
    }
}
