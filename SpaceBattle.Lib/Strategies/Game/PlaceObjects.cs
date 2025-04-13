using Hwdtech;


namespace SpaceBattle.Lib;

public class PlaceObjects : IStrategy
{
    public object Run(params object[] args)
    {
        UObject[] objects = (UObject[]) args[0];
        
        if (args.Count() > 2)
        {
            string placementMethod = (string) args[1];
            if (placementMethod == "Placements.Vertical")
            {
                int vericalOffset = (int) args[2];
                int horizontalPos = (int) args[3];
                for (int i = 0; i < objects.Count(); i++)
                {
                    Vector pos = new Vector(horizontalPos, vericalOffset * i);
                    objects[i].setProperty("position", pos);
                }
            }
            else if (placementMethod == "Placements.PairLike")
            {
                int verticalOffset = (int) args[2];
                int horizontalDif = (int) args[3];
                int horizontalPos = (int) args[4];
                for (int i = 0; i < objects.Count(); i++)
                {
                    Vector pos = new Vector(horizontalPos + horizontalDif * (i % 2), verticalOffset * (i / 2 ));
                    objects[i].setProperty("position", pos);
                }
            }
        }
        else
        {
            int horizontalOffset = (int) args[1];
            for (int i = 0; i < objects.Count(); i++)
            {
                Vector pos = new Vector(horizontalOffset * i, 0);
                objects[i].setProperty("position", pos);
            }
        }
        return new object();
    }
}
