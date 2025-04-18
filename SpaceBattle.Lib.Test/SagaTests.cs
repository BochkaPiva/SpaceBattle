using Xunit;
using SpaceBattle.Lib;
using Hwdtech;
using SpaceBattle.Lib.Strategies;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class SagaTests
{
    public SagaTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
    }

    private void RegisterDependencies()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ICommandFactory", (object[] args) => new CommandFactory()).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MoveCommand", (object[] args) => new MoveCommand((IMovable)args[0])).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ReturnPositionCommand", (object[] args) => new ReturnPositionCommand((IMovable)args[0])).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "WasteFuelCommand", (object[] args) => new WasteFuelCommand((IFuelable)args[0])).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ReturnFuelCommand", (object[] args) => new ReturnFuelCommand((IFuelable)args[0])).Execute();
    }

    [Fact]
    public void SuccessfulSagaExecution()
    {
        // Arrange
        RegisterDependencies();
        var obj = new MockObject();
        var strategy = new SagaStrategy(IoC.Resolve<ICommandFactory>("ICommandFactory"));
        
        // Act
        var saga = strategy.CreateSaga(obj, "MoveCommand", "WasteFuelCommand");
        saga.Execute();
        
        // Assert
        Assert.Equal(new Vector(1, 1), obj.position);
        Assert.Equal(90, obj.Fuel);
    }
    
    [Fact]
    public void SagaRollbackOnError()
    {
        // Arrange
        RegisterDependencies();
        var obj = new MockObject { Fuel = 5 };
        var strategy = new SagaStrategy(IoC.Resolve<ICommandFactory>("ICommandFactory"));
        
        // Act & Assert
        var saga = strategy.CreateSaga(obj, "MoveCommand", "WasteFuelCommand");
        var exception = Assert.Throws<Exception>(() => saga.Execute());
        
        Assert.Equal(new Vector(0, 0), obj.position);
        Assert.Equal(5, obj.Fuel);
    }
}

// Mock объект для тестирования
public class MockObject : IUObject, IMovable, IFuelable
{
    private Dictionary<string, object> properties = new Dictionary<string, object>();
    
    public Vector position { get; set; } = new Vector(0, 0);
    public Vector speed { get; } = new Vector(1, 1);
    public int Fuel { get; set; } = 100;
    public int FuelConsumption { get; } = 10;
    
    public T GetProperty<T>(string key)
    {
        return (T)properties[key];
    }
    
    public void SetProperty<T>(string key, T value)
    {
        properties[key] = value!;
    }
} 