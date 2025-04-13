using Moq;
using Hwdtech.Ioc;
using SpaceBattle.Lib;

namespace SpaceBattleTest;

public class EndMoveCommandTests
{
    [Fact]
    public void EndMoveCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();
        var command = new Mock<ICommand>();
        var registrationStrategy = new Mock<IStrategy>();
        registrationStrategy.Setup(x => x.Run(It.IsAny<object[]>())).Returns(command.Object);
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Command.EmptyCommand", (object[] args) => registrationStrategy.Object.Run(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.DeleteProperty", (object[] args) => registrationStrategy.Object.Run(args)).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.InjectCommand", (object[] args) => registrationStrategy.Object.Run(args)).Execute();
        var moveCommandEndable = new Mock<IMoveCommandEndable>();
        moveCommandEndable.SetupGet(x => x.MoveCommand).Returns(new Mock<ICommand>().Object);
        moveCommandEndable.SetupGet(x => x.Properties).Returns(new List<string>() {"one", "two"});
        ICommand endMoveCommand = new EndMoveCommand(moveCommandEndable.Object);
        endMoveCommand.Execute();
        moveCommandEndable.VerifyAll();
        
    }
}
