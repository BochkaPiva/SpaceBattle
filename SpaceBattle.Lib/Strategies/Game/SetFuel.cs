using Hwdtech;


namespace SpaceBattle.Lib;

public class SetFuel : IStrategy
{
    public object Run(params object[] args)
    {
        UObject obj = (UObject) args[0];
        obj.setProperty("fuel", Convert.ToDouble(args[1]));
        return new object();
    }
}
