namespace SpaceBattle.Lib;
using Hwdtech;

public class InterpretCommand : ICommand 
{
    IMessage message;

    public InterpretCommand(IMessage msg)
    {
        message = msg;
    }

    public void Execute()
    {
        var cmd = IoC.Resolve<SpaceBattle.Lib.ICommand>("CreateCommand", message);

        IoC.Resolve<SpaceBattle.Lib.ICommand>("PushCommand", message.Gameid, cmd).Execute();
    }
}
