using System.Collections.Concurrent;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class ServerThreadTests 
{
    public ServerThreadTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
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

    }
    [Fact]
    public void successfulThreadStart()
    {
        AutoResetEvent waiter = new AutoResetEvent(false);

        var objToMove = new Mock<IMovable>();
        objToMove.SetupProperty(x => x.position);
        objToMove.SetupGet(x => x.speed).Returns(new Vector(-7, 3));
        objToMove.Object.position = new Vector(12, 5);
        var cmd = new MoveCommand(objToMove.Object);

        var releaseThread = new ActionCommand(
            new Action(
                () =>
                {
                    waiter.Set();
                }
            )
        );
    
        IoC.Resolve<ICommand>("Threading.CreateAndStartThread", 0).Execute();

        IoC.Resolve<ICommand>("Threading.SendCommand", 0, cmd).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 0, releaseThread).Execute();

        waiter.WaitOne();
        
        Assert.True(objToMove.Object.position == new Vector(5, 8));
    }
}
