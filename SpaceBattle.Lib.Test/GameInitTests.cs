using Moq;
using Hwdtech;
using Hwdtech.Ioc;


namespace SpaceBattle.Lib.Test;


public class GameInitTests
{
    public GameInitTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        int playerid = 0;
        int objid = 0;
        Dictionary<string, UObject> gameObjects = new Dictionary<string, UObject>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects", (Func<object[], Dictionary<string, UObject>>) (args => gameObjects)).Execute();
        Dictionary<string, object> initProps = new Dictionary<string, object>
        {
            ["numberOfPlayers"] = 2,
        };
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.InitProperties", (Func<object[], Dictionary<string, object>>) (args => initProps)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.AddNewPlayer", (Func<object[], string>) (args => Convert.ToString(playerid++))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects.Empty", (Func<object[], UObject>) (args => new TestObject(new Dictionary<string, object>()))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.Objects.EmptyId", (Func<object[], string>) (args => Convert.ToString(objid++))).Execute(); 
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "General.GetItem", (Func<object[], UObject>) (args => (UObject) new GetItem().Run(args[0]))).Execute();
    }
    
    [Fact]
    public void setFuelTest()
    {
        TestObject obj = new TestObject(new Dictionary<string, object>());
        new SetFuel().Run(obj, 10);
        Assert.True((double) obj.getProperty("fuel") == 10);
    }
    [Fact]
    public void createShipsTests()
    {
        var gameObjects = IoC.Resolve<Dictionary<string, UObject>>("General.Objects");
        new CreateEmptyShips().Run();
        Assert.True(gameObjects.Count() == 6);
        Assert.True((string) gameObjects["0"].getProperty("player") == "0");
        Assert.True((string) gameObjects["1"].getProperty("player") == "0");
        Assert.True((string) gameObjects["2"].getProperty("player") == "0");
        Assert.True((string) gameObjects["3"].getProperty("player") == "1");
        Assert.True((string) gameObjects["4"].getProperty("player") == "1");
        Assert.True((string) gameObjects["5"].getProperty("player") == "1");
    }
    [Fact]
    public void placeObjectsTests()
    {
        var gameObjects = IoC.Resolve<Dictionary<string, UObject>>("General.Objects");
        new CreateEmptyShips().Run();
        var friendlyShips = new UObject[] {
            IoC.Resolve<UObject>("General.GetItem", "0"),
            IoC.Resolve<UObject>("General.GetItem", "1"),
            IoC.Resolve<UObject>("General.GetItem", "2"),
        };
        new PlaceObjects().Run(friendlyShips, "Placements.Vertical", 10, 5);
        Assert.True((Vector) gameObjects["0"].getProperty("position") == new Vector(5, 0));
        Assert.True((Vector) gameObjects["1"].getProperty("position") == new Vector(5, 10));
        Assert.True((Vector) gameObjects["2"].getProperty("position") == new Vector(5, 20));

        var enemyShips = new UObject[] {
            IoC.Resolve<UObject>("General.GetItem", "3"),
            IoC.Resolve<UObject>("General.GetItem", "4"),
            IoC.Resolve<UObject>("General.GetItem", "5"),
        };
        new PlaceObjects().Run(enemyShips, "Placements.PairLike", 15, 5, -5);
        Assert.True((Vector) gameObjects["3"].getProperty("position") == new Vector(-5, 0));
        Assert.True((Vector) gameObjects["4"].getProperty("position") == new Vector(0, 0));
        Assert.True((Vector) gameObjects["5"].getProperty("position") == new Vector(-5, 15));

        new CreateEmptyShips().Run();
        
        var someShips = new UObject[] {
            IoC.Resolve<UObject>("General.GetItem", "6"),
            IoC.Resolve<UObject>("General.GetItem", "7"),
            IoC.Resolve<UObject>("General.GetItem", "8"),
        };
        new PlaceObjects().Run(someShips, 5);
        Assert.True((Vector) gameObjects["6"].getProperty("position") == new Vector(0, 0));
        Assert.True((Vector) gameObjects["7"].getProperty("position") == new Vector(5, 0));
        Assert.True((Vector) gameObjects["8"].getProperty("position") == new Vector(10, 0));
    }
}
