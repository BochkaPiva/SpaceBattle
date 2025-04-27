using System;

namespace SpaceBattle.Lib;

public interface IEndpoint
{
    int ThreadId { get; }
    void Send(object message);
    object Receive();
    void SetRouter(IRouter router);
} 