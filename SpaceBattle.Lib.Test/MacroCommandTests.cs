using Moq;
using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class MacroCommandTests
{
    public MacroCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Dependant_Command", (object[] args) => new Mock<SpaceBattle.Lib.ICommand>().Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GetDependantCommandNames", (object[] args) => new List<string> {"MoveCommand"}).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MoveCommand", (object[] args) => new MoveCommand((IMovable)args[0])).Execute();
    }
    [Fact]
    public void successfulMacro()
    {
        var objToMove = new Mock<IMovable>();
        objToMove.SetupProperty(x => x.position);
        objToMove.SetupGet(x => x.speed).Returns(new Vector(-7, 3));
        objToMove.Object.position = new Vector(12, 5);

        var commandsList = IoC.Resolve<List<string>>("Commands.GetDependantCommandNames", "MoveMacro");
        var commands = new List<ICommand>();
        commandsList.ForEach(cmdName => commands.Add(IoC.Resolve<ICommand>(cmdName, objToMove.Object)));

        var macroCmd = new MacroCommand(commands);
        macroCmd.Execute();
        
        Assert.True(objToMove.Object.position == new Vector(5, 8));
    }
}
