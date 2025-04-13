namespace SpaceBattle.Lib;
public class ExceptionHandler
{
    private Dictionary<string, Dictionary<Type, ICommand>> tree;
    private Dictionary<string, ICommand> defaultTree;
    public ExceptionHandler()
    {
        tree = new();
        tree.Add("Any", new Dictionary<Type, ICommand>());
        defaultTree = new();
    }
    public void Handle(string key, Exception exception)
    {
        ICommand command;
        tree.TryAdd(key, new Dictionary<Type, ICommand>()); 
        if (!tree["Any"].TryGetValue(exception.GetType(), out command!)){
            if (!tree[key].TryGetValue(exception.GetType(), out command!))
            {
                command = defaultTree[key];
            }
        }
        command.Execute();
    }
    public void Add(string key, Exception exception, ICommand command)
    {
        if (!tree.ContainsKey(key))
        {
            tree.Add(key, new());
        }
        tree[key].Add(exception.GetType(), command);
    }
    public void AddDefault(string key, ICommand command)
    {
        defaultTree.Add(key, command);
    }
}
