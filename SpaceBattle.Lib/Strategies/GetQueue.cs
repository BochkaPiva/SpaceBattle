namespace SpaceBattle.Lib;
using Hwdtech;


public class GetQueue : IStrategy  
{
    public object Run (params object[] args)
    {
        string gameid = (string)args[0];
        
        if (!IoC.Resolve<IDictionary<string, Queue<ICommand>>>("GameDictionary").TryGetValue(gameid, out Queue<ICommand>? queue))
        {
            throw new Exception();
        }
        else
        {
            return queue;
        }
    }
}
