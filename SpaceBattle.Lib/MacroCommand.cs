namespace SpaceBattle.Lib;

public class MacroCommand : ICommand
{
    List<ICommand> commands;
    public MacroCommand(List<ICommand> commands)
    {
        this.commands = commands;
    }
    public void Execute()
    {
        commands.ForEach(cmd => cmd.Execute());
    }
}
