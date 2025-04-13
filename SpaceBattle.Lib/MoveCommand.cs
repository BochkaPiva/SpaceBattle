namespace SpaceBattle.Lib;
public class MoveCommand : ICommand
{
    IMovable obj;
    public MoveCommand(IMovable _obj)
    {
        obj = _obj;
    }      
    public void Execute()
    {
        obj.position += obj.speed;
    }
}
