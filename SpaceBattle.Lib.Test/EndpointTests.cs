using Xunit;

namespace SpaceBattle.Lib.Test;

public class EndpointTests
{
    private readonly IEndpoint endpoint;
    private readonly int threadId = 1;

    public EndpointTests()
    {
        endpoint = new Endpoint(threadId);
    }

    [Fact]
    public void ThreadId_ReturnsCorrectValue()
    {
        Assert.Equal(threadId, endpoint.ThreadId);
    }

    [Fact]
    public void Send_MessageIsReceived()
    {
        // Arrange
        var message = "test message";

        // Act
        endpoint.Send(message);
        var receivedMessage = endpoint.Receive();

        // Assert
        Assert.Equal(message, receivedMessage);
    }

    [Fact]
    public void Send_MultipleMessages_ReceivedInOrder()
    {
        // Arrange
        var message1 = "test message 1";
        var message2 = "test message 2";

        // Act
        endpoint.Send(message1);
        endpoint.Send(message2);

        // Assert
        Assert.Equal(message1, endpoint.Receive());
        Assert.Equal(message2, endpoint.Receive());
    }
} 