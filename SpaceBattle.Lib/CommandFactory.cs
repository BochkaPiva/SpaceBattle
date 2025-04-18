using Hwdtech;

namespace SpaceBattle.Lib;

public class CommandFactory : ICommandFactory
{
    public ICommand CreateCommand(string name, IUObject obj)
    {
        return IoC.Resolve<ICommand>(name, obj);
    }
} 