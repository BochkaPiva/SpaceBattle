namespace SpaceBattle.Lib;

public class SagaCommand : ICommand
{
    private readonly List<(ICommand, ICommand)> commands;
    private readonly List<ICommand> executedCommands = new();

    public SagaCommand(List<(ICommand, ICommand)> commands)
    {
        this.commands = commands;
    }

    public void Execute()
    {
        try
        {
            foreach (var (command, _) in commands)
            {
                command.Execute();
                executedCommands.Add(command);
            }
        }
        catch (Exception)
        {
            foreach (var command in executedCommands.AsEnumerable().Reverse())
            {
                var compensationCommand = commands.First(x => x.Item1 == command).Item2;
                compensationCommand.Execute();
            }
            throw;
        }
    }
} 