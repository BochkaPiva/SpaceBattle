namespace SpaceBattle.Lib;

public class DefaultHandler : IStrategy
{
    public object Run(params object[] args)
    {
        var e = (Exception)args[0];
        var cmd = (ICommand)args[1];
        e.Data["cmd"] = cmd;

        throw e; 
    }
}
