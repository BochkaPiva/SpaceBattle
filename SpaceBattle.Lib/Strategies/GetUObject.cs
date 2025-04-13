namespace SpaceBattle.Lib;
using Hwdtech;


public class GetUObject : IStrategy  
{
     public object Run(params object[] args)
    {
        string objectid = (string)args[0];;
        
        if (!IoC.Resolve<IDictionary<string, UObject>>("UObjectDictionary").TryGetValue(objectid, out UObject? uObject))
        {
            throw new Exception();
        }
        else
        {
            return uObject;
        }
    }
}
