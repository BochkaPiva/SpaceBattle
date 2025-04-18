using Hwdtech;

namespace SpaceBattle.Lib.Strategies;

public class SagaStrategy : ISagaStrategy
{
    private readonly ICommandFactory commandFactory;
    
    public SagaStrategy(ICommandFactory commandFactory)
    {
        this.commandFactory = commandFactory;
    }
    
    public ICommand CreateSaga(IUObject obj, params string[] commandNames)
    {
        var commands = new List<(ICommand, ICommand)>();
        foreach (var name in commandNames)
        {
            var directCommand = commandFactory.CreateCommand(name, obj);
            var compensationName = GetCompensationName(name);
            var compensationCommand = commandFactory.CreateCommand(compensationName, obj);
            commands.Add((directCommand, compensationCommand));
        }
        return new SagaCommand(commands);
    }
    
    public object Run(params object[] args)
    {
        if (args.Length < 2)
            throw new ArgumentException("Expected at least 2 arguments: IUObject and command names");

        var obj = args[0] as IUObject ?? throw new ArgumentException("First argument must be IUObject");
        var commandNames = args.Skip(1).Select(a => a?.ToString() ?? throw new ArgumentException("Command name cannot be null")).ToArray();
        
        return CreateSaga(obj, commandNames);
    }
    
    private string GetCompensationName(string commandName)
    {
        return commandName switch
        {
            "MoveCommand" => "ReturnPositionCommand",
            "WasteFuelCommand" => "ReturnFuelCommand",
            _ => throw new Exception($"Unknown command: {commandName}")
        };
    }
} 