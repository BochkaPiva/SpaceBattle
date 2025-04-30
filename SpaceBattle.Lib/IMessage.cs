namespace SpaceBattle.Lib;

public interface IMessage
{
    int GameId { get; }
    string Gameid => GameId.ToString();
    string UObjectid { get; }
    string Typecmd { get; }
    IDictionary<string, object> Args { get; }
} 