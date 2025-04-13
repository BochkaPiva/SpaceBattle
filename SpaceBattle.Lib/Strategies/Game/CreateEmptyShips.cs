using Hwdtech;


namespace SpaceBattle.Lib;

public class CreateEmptyShips : IStrategy
{
    public object Run(params object[] args)
    {
        Dictionary<string, UObject> gameObjects = IoC.Resolve<Dictionary<string, UObject>>("General.Objects");
        Dictionary<string, object> gameParams = IoC.Resolve<Dictionary<string, object>>("Game.InitProperties");
        int numOfPlayers = (int) gameParams["numberOfPlayers"];
        for (int i = 0; i < numOfPlayers; i++)
        {
            string playerId = IoC.Resolve<string>("General.AddNewPlayer");
            for (int j = 0; j < 3; j++)
            {
                UObject newObj = IoC.Resolve<UObject>("General.Objects.Empty");
                newObj.setProperty("player", playerId);
                gameObjects.Add(IoC.Resolve<string>("General.Objects.EmptyId"), newObj);
            }
        }
        return new object();
    }
}
