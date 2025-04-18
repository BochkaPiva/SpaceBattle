namespace SpaceBattle.Lib;

public class ReturnPositionCommand : ICommand
{
    private readonly IMovable obj;
    
    public ReturnPositionCommand(IMovable obj)
    {
        this.obj = obj;
    }
    
    public void Execute()
    {
        obj.position -= obj.speed;
    }
} 