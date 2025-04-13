namespace SpaceBattle.Lib.Test;


public class TestObject : UObject
{
    Dictionary<string, object> properties;
    public TestObject(Dictionary<string, object> props)
    {
        properties = props;
    }
    public object getProperty(string key)
    {
        return properties[key];
    }

    public void setProperty(string key, object value)
    {
        properties[key] = value;
    }
}

public class AdapterTests
{
    [Fact]
    public void StringSourceTest()
    {
        string header = "namespace SpaceBattle.Lib;public class IMovableAdapter : IMovable";

        string openFigBracket = "{";
        string closingFigBracket = "}";

        string targetField = "UObject target;";
        string constructor = "public IMovableAdapter(object _target){target = (UObject) _target;}";

        string posHeader = "public SpaceBattle.Lib.Vector position";
        string posGet = "get => (SpaceBattle.Lib.Vector) target.getProperty(\"position\");";
        string posSet = "set => target.setProperty(\"position\", value);";

        string speedHeader = "public SpaceBattle.Lib.Vector speed";
        string speedGet = "get => (SpaceBattle.Lib.Vector) target.getProperty(\"speed\");";

        string expected = header + openFigBracket + targetField + constructor + posHeader + openFigBracket + posGet + posSet + closingFigBracket + speedHeader + openFigBracket + speedGet + closingFigBracket + closingFigBracket;

        Type? IMovableType = Type.GetType("SpaceBattle.Lib.IMovable, SpaceBattle.Lib", true, true);

        string adapterSource = (string) new CreateAdapterSource(IMovableType!).Run(new object[] {});

        Assert.Equal(expected, adapterSource);
    }
}
