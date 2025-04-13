using Moq;

namespace SpaceBattle.Lib.Test;

public class vectorTests
{
    [Fact]
    public void setIndex()
    {
        Vector v = new Vector(0, 0, 0);
        v[0] = 1;
        Assert.Equal(1, v[0]);
    }
    [Fact]
    public void unsuccessfulSumdifferentDimensionVectors()
    {
        Vector v1 = new Vector(0, 0, 0);
        Vector v2 = new Vector(0, 0);
        Assert.Throws<Exception>(() => v1 + v2);
    }
    [Fact]
    public void unsuccessfulSubtractionDifferentDimensionVectors()
    {
        Vector v1 = new Vector(0, 0, 0);
        Vector v2 = new Vector(0, 0);
        Assert.Throws<Exception>(() => v1 - v2);
    }
    [Fact]
    public void successfulSum()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(3, 2, 3);
        Assert.Equal(new Vector(4, 4, 6), v1 + v2);
    }
    [Fact]
    public void successfulSubtract()
    {
        Vector v1 = new Vector(3, 4, 5);
        Vector v2 = new Vector(2, 1, 0);
        Assert.Equal(new Vector(1, 3, 5), v1 - v2);
    }
    [Fact]
    public void successfulLeftHandScaling()
    {
        Vector v = new Vector(1, 2, 3);
        int s = 5;
        Assert.Equal(new Vector(1*s, 2*s, 3*s), s*v);
    }
    [Fact]
    public void successfulRightHandScaling()
    {
        Vector v = new Vector(1, 2, 3);
        int s = 5;
        Assert.Equal(new Vector(1*s, 2*s, 3*s), v*s);
    }
    [Fact]
    public void vectorsAreNotEqualValuesNotMatching()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(3, 2, 1);
        Assert.True(v1 != v2);
    }
    [Fact]
    public void vectorsAreNotEqualDimensionsNotMatching()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(1, 2);
        Assert.False(v1 == v2);
    }
    [Fact]
    public void vectorsAreEqual()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(1, 2, 3);
        Assert.True(v1 == v2);
    }
    [Fact]
    public void vectorsAreTheSameOne()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = v1;
        Assert.True(v1 == v2);
    }
    [Fact]
    public void vectorsAreNotTheSameByNullValue()
    {
        Vector v1 = new Vector(1, 2, 3);
        Assert.False(v1 is null);
    }
    [Fact]
    public void vectorsAreEqualByHashCode()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(1, 2, 3);
        Assert.True(v1 == v2);
    }
    [Fact]
    public void vectorsAreNotEqualByHashCode()
    {
        Vector v1 = new Vector(1, 2, 3);
        Vector v2 = new Vector(3, 2, 1);
        Assert.False(v1 == v2);
    }
    [Fact]
    public void vectorAndObjectAreNotEqual()
    {
        Vector v1 = new Vector(1, 2, 3);
        double s = 3.5;
        Assert.False(v1.Equals(s));
    }
    [Fact]
    public void successfulVectorToStringConversion()
    {
        Vector v1 = new Vector(1, 2, 3);
        Assert.Equal("Vector(1, 2, 3)", v1.ToString());
    }
    [Fact]
    public void getHashCodeThrowsException()
    {
        Vector v1 = new Vector(1, 2, 3);
        Assert.Throws<Exception>(() => v1.GetHashCode());
    }
}
