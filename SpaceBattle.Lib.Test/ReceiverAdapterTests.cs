using System.Collections.Concurrent;
using Moq;

namespace SpaceBattle.Lib.Test;

public class ReceiverAdapterTests 
{
    [Fact]
    public void isEmptyTestReturnsTrue()
    {
        var ra = new ReceiverAdapter(new BlockingCollection<ICommand>());
        Assert.True(ra.isEmpty());
    }
    [Fact]
    public void isEmptyTestReturnsFalse()
    {
        var queue = new BlockingCollection<ICommand>();
        queue.Add(new Mock<ICommand>().Object);

        var ra = new ReceiverAdapter(queue);

        Assert.False(ra.isEmpty());
    }
    [Fact]
    public void successfulReceiveCommand()
    {
        var queue = new BlockingCollection<ICommand>();
        
        var objToMove = new Mock<IMovable>();
        objToMove.SetupProperty(x => x.position);
        objToMove.SetupGet(x => x.speed).Returns(new Vector(-7, 3));
        objToMove.Object.position = new Vector(12, 5);
        var cmd = new MoveCommand(objToMove.Object);
        
        queue.Add(cmd);

        var ra = new ReceiverAdapter(queue);

        ra.Receive().Execute();

        Assert.True(objToMove.Object.position == new Vector(5, 8));
    }
}
