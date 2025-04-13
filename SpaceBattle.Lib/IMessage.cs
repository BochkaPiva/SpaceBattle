namespace SpaceBattle.Lib;


public interface IMessage
{
    public string Gameid { get; }
    public string UObjectid { get; }
    public string Typecmd { get; }
    public IDictionary<string, object> Args { get; }
}
