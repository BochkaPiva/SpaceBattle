using Hwdtech;

namespace SpaceBattle.Lib;

public class CreateMacro : IStrategy
{
    public object Run(object[] args)
    {
        List<string> dependantCommands = IoC.Resolve<List<string>>("Commands.GetDependantCommandNames", args[0]);
        List<ICommand> commands = new List<ICommand>();
        dependantCommands.ForEach(cmd => commands.Add(IoC.Resolve<ICommand>(cmd, args[1])));
        return new MacroCommand(commands);
    }
}
