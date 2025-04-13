using Hwdtech;

namespace SpaceBattle.Lib;
public interface IMoveCommandEndable
{
    UObject Object { get; }
    ICommand MoveCommand { get; }
    IList<string> Properties { get; }
}
public class EndMoveCommand : ICommand
{
    public IMoveCommandEndable obj;
    public EndMoveCommand(IMoveCommandEndable obj)
    {
        this.obj = obj;
    }
    public void Execute()
    {
        ICommand EndCommand = IoC.Resolve<ICommand>("Command.EmptyCommand");
        foreach (var property in obj.Properties) {
            IoC.Resolve<ICommand>("Game.DeleteProperty", obj.Object, property).Execute();
            }
    IoC.Resolve<ICommand>("Game.InjectCommand", obj.MoveCommand, EndCommand).Execute();
    }
}
