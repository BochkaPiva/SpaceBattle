using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SpaceBattle.Lib;

public class SagaCommand : ICommand
{
    private readonly List<(ICommand, ICommand)> commands;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 100;

    public SagaCommand(List<(ICommand, ICommand)> commands)
    {
        this.commands = commands;
    }

    public void Execute()
    {
        var executedCommands = new List<ICommand>();
        
        try
        {
            foreach (var (command, _) in commands)
            {
                var retryCount = 0;
                var success = false;
                
                while (!success && retryCount < MaxRetries)
                {
                    try
                    {
                        command.Execute();
                        executedCommands.Add(command);
                        success = true;
                    }
                    catch (Exception)
                    {
                        retryCount++;
                        if (retryCount == MaxRetries)
                        {
                            throw;
                        }
                        Thread.Sleep(RetryDelayMs);
                    }
                }
            }
        }
        catch (Exception)
        {
            foreach (var command in executedCommands.AsEnumerable().Reverse())
            {
                var retryCount = 0;
                var success = false;
                
                while (!success && retryCount < MaxRetries)
                {
                    try
                    {
                        var compensationCommand = commands.First(x => x.Item1 == command).Item2;
                        compensationCommand.Execute();
                        success = true;
                    }
                    catch (Exception)
                    {
                        retryCount++;
                        if (retryCount == MaxRetries)
                        {
                            throw;
                        }
                        Thread.Sleep(RetryDelayMs);
                    }
                }
            }
            throw;
        }
    }
} 