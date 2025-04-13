using Moq;

namespace SpaceBattle.Lib.Test;

public class SetPropertyTests
{
    [Fact]
    public void successfullSetPropertyRun()
    {
        var obj = new Mock<UObject>();
        var key = new string("");
        var value = new Mock<object>();

        var strategy = new SetProperty();

        strategy.Run(obj.Object, key, value.Object);
    }
    [Fact]
    public void unsuccessfullSetPropertyRunNoValueGiven()
    {
        var obj = new Mock<UObject>();
        var key = new string("");

        var strategy = new SetProperty();

        Assert.Throws<IndexOutOfRangeException>(() => strategy.Run(obj.Object, key));
    }
    [Fact]
    public void unsuccessfullSetPropertyRunNoKeyGiven()
    {
        var obj = new Mock<UObject>();
        var value = new Mock<object>();


        var strategy = new SetProperty();

        Assert.Throws<InvalidCastException>(() => strategy.Run(obj.Object, value.Object));
    }
    [Fact]
    public void unsuccessfullSetPropertyRunNoObjectGiven()
    {
        var key = new string("");
        var value = new Mock<object>();

        var strategy = new SetProperty();

        Assert.Throws<InvalidCastException>(() => strategy.Run(key, value.Object));
    }
    [Fact]
    public void unsuccessfullSetPropertyRunUnableToGetSetPropertyFromObj()
    {
        var obj = new Mock<UObject>();
        var key = new string("");
        var value = new Mock<object>();
        obj.Setup(o => o.setProperty(It.IsAny<string>(), It.IsAny<object>())).Throws<Exception>();

        var strategy = new SetProperty();

        Assert.Throws<Exception>(() => strategy.Run(obj.Object, key, value.Object));
    }
}
