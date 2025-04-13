namespace SpaceBattle.Lib.Test;
using Moq;

public class RotateTest
{
    [Fact]
    public void Test1()
    {
        Mock<IRotatable> obj = new();
        obj.SetupProperty(x => x.Angle, new(45));
        obj.SetupGet(x => x.AngleSpeed).Returns(new Vector(90));
        RotateCommand rotate = new(obj.Object);

        rotate.Execute();

        Assert.Equal(new(135), obj.Object.Angle);
    }
    [Fact]
    public void Test2()
    {
        Mock<IRotatable> obj = new();
        obj.SetupGet(x => x.Angle).Throws<Exception>();
        RotateCommand rotate = new(obj.Object);
        Assert.Throws<Exception>(()=>rotate.Execute());

    }
    [Fact]
    public void Test3()
    {
        
        Mock<IRotatable> obj = new();
        obj.SetupProperty(x => x.Angle, new(45));
        obj.SetupGet(x => x.AngleSpeed).Throws<Exception>();
        RotateCommand rotate = new(obj.Object);
        Assert.Throws<Exception>(()=>rotate.Execute());

    }
    [Fact]
    public void Test4()
    {
        Mock<IRotatable> obj = new();
        obj.SetupGet(x => x.Angle).Returns(new Vector(0));
        obj.SetupSet(x => x.Angle = It.IsAny<Vector>()).Throws(new Exception());
        obj.SetupGet(x => x.AngleSpeed).Returns(new Vector(90));
        RotateCommand rotate = new(obj.Object);

        Assert.Throws<Exception>(rotate.Execute);
    }
}
