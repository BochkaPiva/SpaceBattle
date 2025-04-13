using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib.Test;


public class CreateMacroTests
{
    public CreateMacroTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GetDependantCommandNames", (object[] args) => new List<string> {"MoveCommand"}).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.GetCommand.WithArgs", (object[] args) => new Mock<IStrategy>().Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MoveCommand", (object[] args) => new MoveCommand((IMovable)args[0])).Execute();
    }
    [Fact]
    public void successfulCreateMacro()
    {
        var objToMacro = new Mock<IMovable>();
        objToMacro.SetupProperty(x => x.position);
        objToMacro.SetupGet(x => x.speed).Returns(new Vector(-7, 3));
        objToMacro.Object.position = new Vector(12, 5);

        var newMacro = new Mock<CreateMacro>();
        ICommand MoveMacro = (ICommand) newMacro.Object.Run(new object[] {"MoveMacro", objToMacro.Object});
        MoveMacro.Execute();

        Assert.True(objToMacro.Object.position == new Vector(5, 8));
    }
}
