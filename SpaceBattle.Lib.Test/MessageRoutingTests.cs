using System.Collections.Concurrent;
using Hwdtech;
using Hwdtech.Ioc;
using Xunit;

namespace SpaceBattle.Lib.Test;

public class MessageRoutingTests
{
    object scope;
    public MessageRoutingTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.ActionCommand", (object[] args) => new ActionCommand((Action)args[0])).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.InterpretMessage", (object[] args) => (ICommand)args[0]).Execute();

        var serverThreads = new Dictionary<int, (ServerThread, SenderAdapter)>();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.ServerThreads", (object[] args) => serverThreads).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.CreateAndStartThread", (object[] args) => new ActionCommand(new Action(
            () =>
            {
                var queue = new BlockingCollection<ICommand>();
                
                if (args.Length == 2)
                {
                    queue.Add(IoC.Resolve<ICommand>("Commands.ActionCommand", (Action)args[1]));
                }

                var receiverQueue = new ReceiverAdapter(queue);
                var senderQueue = new SenderAdapter(queue);

                IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")
                .Add((int)args[0], (new ServerThread(receiverQueue), senderQueue));

                IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[(int)args[0]].Item1.Start();
            }
        ))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.SendCommand", (object[] args) => new ActionCommand(new Action(
            () =>
            {
                int threadId = (int)args[0];
                ICommand cmd = (ICommand)args[1];
                var threads = IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads");
                threads[threadId].Item2.Send((object)cmd);
            }
        ))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.GetThreadId", (object[] args) =>
        {
            var thread = (ServerThread)args[0];
            return (object) IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")
            .Where(x => x.Value.Item1 == thread).ToList()[0].Key;
        }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Commands.SetScopeInCurrentThreadCommand", (object[] args) => 
        {
            var scope = (object)args[0];
            return new ActionCommand(() => {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
            });
        }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.SetScope", (object[] args) => 
        {
            return IoC.Resolve<ICommand>("Commands.SetScopeInCurrentThreadCommand", args[0]);
        }
        ).Execute();

        var routingService = new RoutingService();
        new ActionCommand(() => 
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "RoutingService.Get", (object[] args) => routingService).Execute()
        ).Execute();

        var routers = new Dictionary<int, IRouter>();
        new ActionCommand(() => 
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Router.Create", (object[] args) => 
            {
                var threadId = (int)args[0];
                var router = new Router(routingService, threadId);
                routingService.RegisterRouter(threadId, router);
                routers[threadId] = router;
                return router;
            }).Execute()
        ).Execute();

        new ActionCommand(() => 
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Router.Get", (object[] args) => 
            {
                var threadId = (int)args[0];
                if (!routers.ContainsKey(threadId))
                {
                    routers[threadId] = IoC.Resolve<IRouter>("Router.Create", threadId);
                }
                return routers[threadId];
            }).Execute()
        ).Execute();

        new ActionCommand(() => 
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Endpoint.Create", (object[] args) => 
            {
                var threadId = (int)args[0];
                var endpoint = new Endpoint(threadId);
                var router = IoC.Resolve<IRouter>("Router.Get", threadId);
                endpoint.SetRouter(router);
                return endpoint;
            }).Execute()
        ).Execute();
    }

    [Fact]
    public void Message_RoutedToGameInSameThread()
    {
        var threadId = 1;
        var gameId = 1;
        var game = new Game(gameId);
        var routingService = IoC.Resolve<IRoutingService>("RoutingService.Get");
        var router = IoC.Resolve<IRouter>("Router.Get", threadId);
        var endpoint = IoC.Resolve<IEndpoint>("Endpoint.Create", threadId);

        IoC.Resolve<ICommand>("Threading.CreateAndStartThread", threadId, new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        })).Execute();

        Thread.Sleep(100);
        router.RegisterGame(game);
        Thread.Sleep(100);

        var message = new TestMessage(gameId);
        endpoint.Send(message);
        Thread.Sleep(100);

        Assert.True(game.HasProcessedMessage(message));
    }

    [Fact]
    public void Message_RoutedToGameInDifferentThread()
    {
        var threadId1 = 1;
        var threadId2 = 2;
        var gameId = 1;
        var game = new Game(gameId);
        var routingService = IoC.Resolve<IRoutingService>("RoutingService.Get");
        
        var router1 = IoC.Resolve<IRouter>("Router.Get", threadId1);
        var endpoint1 = IoC.Resolve<IEndpoint>("Endpoint.Create", threadId1);
        
        var router2 = IoC.Resolve<IRouter>("Router.Get", threadId2);
        var endpoint2 = IoC.Resolve<IEndpoint>("Endpoint.Create", threadId2);

        router2.RegisterGame(game);

        var message = new TestMessage(gameId);
        endpoint1.Send(message);

        Assert.True(game.HasProcessedMessage(message));
    }

    private class TestMessage : IMessage
    {
        public int GameId { get; }
        public string UObjectid => "test_object";
        public string Typecmd => "test_command";
        public IDictionary<string, object> Args { get; } = new Dictionary<string, object>();

        public TestMessage(int gameId)
        {
            GameId = gameId;
        }
    }
} 