namespace SpaceBattle.Lib;

public class ReturnFuelCommand : ICommand
{
    private readonly IFuelable obj;
    
    public ReturnFuelCommand(IFuelable obj)
    {
        this.obj = obj;
    }
    
    public void Execute()
    {
        obj.Fuel += obj.FuelConsumption;
    }
} 