namespace SpaceBattle.Lib;

public interface ICommandFactory
{
    ICommand CreateCommand(string name, IUObject obj);
} 