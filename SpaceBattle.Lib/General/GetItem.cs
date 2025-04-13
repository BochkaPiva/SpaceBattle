using Hwdtech;


namespace SpaceBattle.Lib;

public class GetItem : IStrategy
{
    public object Run(params object[] args)
    {
        IoC.Resolve<Dictionary<string, UObject>>("General.Objects").TryGetValue((string) args[0], out UObject? obj);

        if (obj != null)
        {
            return obj;
        }
        throw new Exception();
    }
}
