namespace SpaceBattle.Lib;

public class SetProperty : IStrategy
{
    public object Run(params object[] args)
    {
        ((UObject)args[0]).setProperty((string)args[1], args[2]);
        return new object();
    }
}
