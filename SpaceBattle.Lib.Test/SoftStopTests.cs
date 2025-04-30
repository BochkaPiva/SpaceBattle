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
        var waiter = new ManualResetEventSlim(false);
        var commandsFinished = new ManualResetEventSlim(false);

        var cmd = IoC.Resolve<ICommand>("Commands.ActionCommand", new Action(
            () =>
            {
                Thread.Sleep(100);
                waiter.Set();
            }
        ));

        // Create and start the thread
        IoC.Resolve<ICommand>("Threading.CreateAndStartThread", 2, new Action(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        })).Execute();

        // Wait for thread to be ready
        Thread.Sleep(100);

        var thread = IoC.Resolve<Dictionary<int, (ServerThread, SenderAdapter)>>("Threading.ServerThreads")[2].Item1;
        var softStop = new SoftStopThreadCommand(
            thread,
            new Action(() => commandsFinished.Set())
        );

        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();

        IoC.Resolve<ICommand>("Threading.SendCommand", 2, softStop).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();
        IoC.Resolve<ICommand>("Threading.SendCommand", 2, cmd).Execute();

        var threadReceiver = thread.queue;

        waiter.Wait();
        commandsFinished.Wait(1000);
        
        Assert.True(threadReceiver.isEmpty());
    }
}
