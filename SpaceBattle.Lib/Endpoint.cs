using System;
using System.Threading;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib;

public class Endpoint : IEndpoint
{
    private readonly int threadId;
    private readonly BlockingCollection<object> messages;
    private IRouter? router;

    public Endpoint(int threadId)
    {
        this.threadId = threadId;
        this.messages = new BlockingCollection<object>();
    }

    public int ThreadId => threadId;

    public void Send(object message)
    {
        if (message is IMessage msg && router != null)
        {
            router.Route(msg);
        }
        else
        {
            messages.Add(message);
        }
    }

    public object Receive()
    {
        return messages.Take();
    }

    public void SetRouter(IRouter router)
    {
        this.router = router;
    }
} 