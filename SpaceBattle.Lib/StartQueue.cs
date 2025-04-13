using Hwdtech;
using System.Diagnostics;
namespace SpaceBattle.Lib;



public class StartQueue : ICommand
{
    private Queue<ICommand> queue;

    public StartQueue(Queue<ICommand> queue)
    {
        this.queue = queue;
    }


    public void Execute()
    {
        var stopwatch = new Stopwatch();

        stopwatch.Start();

        while (stopwatch.ElapsedMilliseconds <= IoC.Resolve<int>("GetQuantum"))
        {
            var cmd = IoC.Resolve<ICommand>("QueueDequeue", queue);
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<IStrategy>("ExceptionHandler", e, cmd).Run(e, cmd);
            }
        }
        stopwatch.Stop();
    }
}
