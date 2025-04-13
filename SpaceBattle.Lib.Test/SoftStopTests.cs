using System.Collections.Concurrent;
using Moq;
using Hwdtech;
using Hwdtech.Ioc;

namespace SpaceBattle.Lib.Test;

public class SoftStopTests
{
    object scope;
    public SoftStopTests()
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

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.HardStop", (object[] args) => 
        {
            Action task = new Action(() => {});
            if (args.Length == 2)
            {
                task = (Action)args[1];
            }
            return new HardStopThreadCommand(
            IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[(int)args[0]].Item1,
            task,
            IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[(int)args[0]].Item2);
        }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Threading.SoftStop", (object[] args) => 
        {
            Action task = new Action(() => {});
            if (args.Length > 1)
            {
                task = (Action)args[1];
            }
            return new SoftStopThreadCommand(
            IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[(int)args[0]].Item1,
            task
            );
        }
        ).Execute();
    }

    [Fact]
    public void successfulSoftStop()
    {
        AutoResetEvent waiter = new AutoResetEvent(false);

        var objToMove = new Mock<IMovable>();
        objToMove.SetupProperty(x => x.position);
        objToMove.SetupGet(x => x.speed).Returns(new Vector(-7, 3));
        objToMove.Object.position = new Vector(12, 5);
        
        var cmd = new MoveCommand(objToMove.Object);
        
        IoC.Resolve<ICommand>("Threading.CreateAndStartThread", 2, new Action(
            () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
            }
        )).Execute();

        ICommand softStop = IoC.Resolve<ICommand>("Threading.SoftStop", 2, new Action(
            () =>
            {
                waiter.Set();
            }
        ));

        var releaseThread = new ActionCommand(
            () =>
            {
                waiter.Set();
            }
        );

        var thread0 = IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[2].Item1;

        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();

        IoC.Resolve<ICommand>("Threading.SendCommand", 2, softStop).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();

        var threadReceiver = IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[2].Item1.queue;

        waiter.WaitOne();
        
        Assert.True(threadReceiver.isEmpty());
    }
}
