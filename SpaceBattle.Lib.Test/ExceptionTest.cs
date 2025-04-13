using SpaceBattle.Lib;
using Moq;

namespace UnitTests;
public class ExceptionTest
{
    [Fact]
    public void successfulHandle()
    {
        double number = 1;
        ExceptionHandler handler = new();
        Mock<ICommand> command = new();
        command.Setup(x => x.Execute()).Callback(() => number = 0);
        handler.Add("Divide", new DivideByZeroException(), command.Object);
        try
        {
            throw new DivideByZeroException();
        }
        catch(Exception exception)
        {
            handler.Handle("Divide", exception);
        }           
        Assert.Equal(0, number);
    }
    [Fact]
    public void successfulDefaultHandle()
    {
        double number = 1;
        ExceptionHandler handler = new();
        Mock<ICommand> command = new();
        command.Setup(x => x.Execute()).Callback(() => number = 0);
        handler.AddDefault("Divide", command.Object);
        try
        {
            throw new DivideByZeroException();
        }
        catch(Exception exception)
        {
            handler.Handle("Divide", exception);
        }
        Assert.Equal(0, number);
    }
    [Fact]
    public void successfulUniversalHandle()
    {
        double number = 0;
        ExceptionHandler handler = new();
        Mock<ICommand> command1 = new();
        Mock<ICommand> command2 = new();
        command1.Setup(x => x.Execute()).Callback(() => number = 0);
        command2.Setup(x => x.Execute()).Callback(() => number = 10);
        handler.Add("Any", new DivideByZeroException(), command1.Object);
        handler.AddDefault("Divide", command2.Object);
        try
        {
            throw new DivideByZeroException();
        }
        catch(Exception exception)
        {
            handler.Handle("Divide", exception);
        }            
        Assert.Equal(0, number);
    }      
}
