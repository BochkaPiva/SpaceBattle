namespace SpaceBattle.Lib.Test;
using Hwdtech;
using Hwdtech.Ioc;
using Moq;


public class TestInterpretCommand
{

    public TestInterpretCommand()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetQueue", (object[] args) => new GetQueue().Run(args)).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "GetUObject", (object[] args) => new GetUObject().Run(args)).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "CreateCommand", (object[] args) => new CreateCommand().Run(args)).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "PushCommand", (object[] args) => new PushCommand().Run(args)).Execute();
    }

    [Fact]
    public void CompletedPush()
    {
        Dictionary<string, Queue<SpaceBattle.Lib.ICommand>> gameDictionary = new Dictionary<string, Queue<SpaceBattle.Lib.ICommand>>();
        Dictionary<string, UObject> uobjectDictionary = new Dictionary<string, UObject>();

        IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();
        Mock<UObject> mockUObject = new Mock<UObject>();

        mockUObject.Setup(x => x.setProperty(It.IsAny<string>(), It.IsAny<object>()));

        gameDictionary.Add("1", new Queue<SpaceBattle.Lib.ICommand>());

        uobjectDictionary.Add("1", mockUObject.Object);

        Mock<IMessage> mockMessage = new Mock<IMessage>();
        mockMessage.SetupGet(x => x.Gameid).Returns("1");
        mockMessage.SetupGet(x => x.Typecmd).Returns("Test");
        mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
        mockMessage.SetupGet(x => x.UObjectid).Returns("1");

        IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

        var intepretcmd = new InterpretCommand(mockMessage.Object);
        intepretcmd.Execute();

        Assert.True(gameDictionary["1"].Count == 1);
    }

    [Fact]
    public void GetGameException()
    {
        Dictionary<string, Queue<SpaceBattle.Lib.ICommand>> gameDictionary = new Dictionary<string, Queue<SpaceBattle.Lib.ICommand>>();
        Dictionary<string, UObject> uobjectDictionary = new Dictionary<string, UObject>();

        IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();

        Mock<UObject> mockUObject = new Mock<UObject>();
        mockUObject.Setup(x => x.setProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

        gameDictionary.Add("1", new Queue<SpaceBattle.Lib.ICommand>());

        uobjectDictionary.Add("1", mockUObject.Object);

        Mock<IMessage> mockMessage = new Mock<IMessage>();
        mockMessage.SetupGet(x => x.Gameid).Returns("14");
        mockMessage.SetupGet(x => x.Typecmd).Returns("Test");
        mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
        mockMessage.SetupGet(x => x.UObjectid).Returns("1");

        IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

        var intepretcmd = new InterpretCommand(mockMessage.Object);
        Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
    }

    [Fact]
    public void GetUObjectException()
    {
        Dictionary<string, Queue<SpaceBattle.Lib.ICommand>> gameDictionary = new Dictionary<string, Queue<SpaceBattle.Lib.ICommand>>();
        Dictionary<string, UObject> uobjectDictionary = new Dictionary<string, UObject>();

        IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();

        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();

        Mock<UObject> mockUObject = new Mock<UObject>();
        mockUObject.Setup(x => x.setProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

        gameDictionary.Add("1", new Queue<SpaceBattle.Lib.ICommand>());

        uobjectDictionary.Add("1", mockUObject.Object);

        Mock<IMessage> mockMessage = new Mock<IMessage>();
        mockMessage.SetupGet(x => x.Gameid).Returns("1");
        mockMessage.SetupGet(x => x.Typecmd).Returns("Test");
        mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
        mockMessage.SetupGet(x => x.UObjectid).Returns("14");

        IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

        var intepretcmd = new InterpretCommand(mockMessage.Object);
        Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
    }

    [Fact]
    public void GetMessageParamException()
    {
        Dictionary<string, Queue<SpaceBattle.Lib.ICommand>> gameDictionary = new Dictionary<string, Queue<SpaceBattle.Lib.ICommand>>();
        Dictionary<string, UObject> uobjectDictionary = new Dictionary<string, UObject>();

        IoC.Resolve<ICommand>("IoC.Register", "GameDictionary", (object[] args) => gameDictionary).Execute();
        IoC.Resolve<ICommand>("IoC.Register", "UObjectDictionary", (object[] args) => uobjectDictionary).Execute();
        Mock<SpaceBattle.Lib.ICommand> mockCommand = new Mock<SpaceBattle.Lib.ICommand>();

        Mock<UObject> mockUObject = new Mock<UObject>();
        mockUObject.Setup(x => x.setProperty(It.IsAny<string>(), It.IsAny<object>())).Verifiable();

        Mock<IMessage> mockMessage = new Mock<IMessage>();
        mockMessage.SetupGet(x => x.Gameid).Throws(new Exception());
        mockMessage.SetupGet(x => x.Typecmd).Returns("Test");
        mockMessage.SetupGet(x => x.Args).Returns(new Dictionary<string, object> { { "Test", 1 } });
        mockMessage.SetupGet(x => x.UObjectid).Returns("1");

        IoC.Resolve<ICommand>("IoC.Register", "CommandTest", (object[] args) => mockCommand.Object).Execute();

        var intepretcmd = new InterpretCommand(mockMessage.Object);
        Assert.Throws<Exception>(() => { intepretcmd.Execute(); });
    }
}
