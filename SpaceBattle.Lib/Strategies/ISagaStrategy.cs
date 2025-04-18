namespace SpaceBattle.Lib.Strategies;

public interface ISagaStrategy : IStrategy
{
    ICommand CreateSaga(IUObject obj, params string[] commandNames);
} 