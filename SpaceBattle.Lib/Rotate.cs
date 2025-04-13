namespace SpaceBattle.Lib;
public class RotateCommand: ICommand
{
    private IRotatable _obj;

    public RotateCommand(IRotatable obj)
    {
        _obj = obj;
    }
    public void Execute()
    {
        _obj.Angle = _obj.Angle + _obj.AngleSpeed;
    }

}
