using Hwdtech;


namespace SpaceBattle.Lib;

public class CreateCommand : IStrategy  
{
    public object Run(params object[] args)
    {
        var message = (IMessage)args[0];

        var UObject = IoC.Resolve<UObject>("GetUObject", message.UObjectid);
        
        message.Args.ToList().ForEach(x => UObject.setProperty(x.Key, x.Value));
        return IoC.Resolve<SpaceBattle.Lib.ICommand>("Command" + message.Typecmd, UObject);
    }
}
