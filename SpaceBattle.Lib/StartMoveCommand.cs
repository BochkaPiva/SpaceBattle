using Hwdtech;


namespace SpaceBattle.Lib;
public class StartMoveCommand : ICommand
{
    IStartingMoveCommand initializer;
    public StartMoveCommand(IStartingMoveCommand obj)
    {
        initializer = obj;
    }
    public void Execute()
    {
        initializer.order.ToList().ForEach(o => IoC.Resolve<IStrategy>("General.SetProperty", initializer.objToMove, o.Key, o.Value).Run());
        IMovable objToMove = IoC.Resolve<IMovable>("Adapters.IMovable", initializer.objToMove);
        ICommand moveCommand = IoC.Resolve<ICommand>("Commands.MoveCommand", objToMove);
        IoC.Resolve<ICommand>("Queue.Push").Execute();
    }
}
