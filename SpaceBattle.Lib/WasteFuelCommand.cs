namespace SpaceBattle.Lib;

public class WasteFuelCommand : ICommand
{
    private readonly IFuelable obj;
    
    public WasteFuelCommand(IFuelable obj)
    {
        this.obj = obj;
    }
    
    public void Execute()
    {
        if (obj.Fuel < obj.FuelConsumption)
        {
            throw new Exception("Not enough fuel");
        }
        obj.Fuel -= obj.FuelConsumption;
    }
} 