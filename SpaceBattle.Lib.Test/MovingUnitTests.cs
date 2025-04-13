using Moq;

namespace SpaceBattle.Lib.Test;

public class moveObjectTests
{
    [Fact]
    public void successfulObjectMove()
    {
        var mockObj = new Mock<IMovable>();
        mockObj.Setup(x => x.position).Returns(new Vector(12, 5)).Verifiable();
        mockObj.SetupGet(x => x.speed).Returns(new Vector(-7, 3)).Verifiable();
        var com = new MoveCommand(mockObj.Object);
        com.Execute();
        mockObj.VerifySet(x => x.position = new Vector(5, 8));
    }
    [Fact]
    public void unsuccessfulObjectMoveUnableToReadPosition()
    {
        var mockObj = new Mock<IMovable>();
        mockObj.SetupGet(x => x.position).Throws<Exception>().Verifiable();
        mockObj.SetupGet(x => x.speed).Returns(new Vector(-7, 3)).Verifiable();
        var com = new MoveCommand(mockObj.Object);
        Assert.Throws<Exception>(() => com.Execute());
    }
    [Fact]
    public void unsuccessfulObjectMoveUnableToReadSpeed()
    {
        var mockObj = new Mock<IMovable>();
        mockObj.SetupGet(x => x.position).Returns(new Vector(12, 5)).Verifiable();
        mockObj.SetupGet(x => x.speed).Throws<Exception>().Verifiable();
        var com = new MoveCommand(mockObj.Object);
        Assert.Throws<Exception>(() => com.Execute());
    }
    [Fact]
    public void unsuccessfulObjectMoveUnableToChangePosition()
    {
        var mockObj = new Mock<IMovable>();
        mockObj.SetupGet(x => x.position).Returns(new Vector(12, 5)).Verifiable();
        mockObj.SetupGet(x => x.speed).Returns(new Vector(-7, 3, 3)).Verifiable();
        var com = new MoveCommand(mockObj.Object);
        Assert.Throws<Exception>(() => com.Execute());
    }
}

