
using System;
using System.Collections.Concurrent;
using System.Threading;
using Communication;
using ConsoleCommSimulator.Data;

namespace ConsoleCommSimulator
{
  public partial class ConsoleSimulator
  {
    public CanBusEventArgs CanBusOneEventArgs { get; set; }
    public CanBusEventArgs CanBusTwoEventArgs { get; set; }

    public event EventHandler<CanBusEventArgs> MessageReceivedOne;
    public event EventHandler<CanBusEventArgs> MessageReceivedTwo;

    private readonly ConcurrentQueue<CanBusEventArgs> _canBusOneMessageQueue = new ConcurrentQueue<CanBusEventArgs>();
    private readonly ConcurrentQueue<CanBusEventArgs> _canBusTwoMessageQueue = new ConcurrentQueue<CanBusEventArgs>();

    private Thread _canBusOneMessageThread;
    private Thread _canBusTwoMessageThread;

    private volatile bool _disposed = false;

    private void ProcessCanBusMessage(CanBusId canBusId)
    {
      var messageQueue = canBusId == CanBusId.CanBus1 
                        ?_canBusOneMessageQueue 
                        : _canBusTwoMessageQueue;

      while (!_disposed)
      {
        if (messageQueue.TryDequeue(out CanBusEventArgs newCanbusEventArgs))
        {
          if (canBusId == CanBusId.CanBus1)
          {
            CanBusOneEventArgs = newCanbusEventArgs; 
          }
          else
          {
            CanBusTwoEventArgs = newCanbusEventArgs;
          }

          var eventHandler = canBusId == CanBusId.CanBus1
            ? MessageReceivedOne
            : MessageReceivedTwo;

          eventHandler?.Invoke(this, canBusId == CanBusId.CanBus1 ? CanBusOneEventArgs : CanBusTwoEventArgs);
        }

        Thread.Sleep(0);
      }
    }

    private void CreateCanBusProcessThreads()
    {
      _canBusOneMessageThread = new Thread(() => ProcessCanBusMessage(CanBusId.CanBus1));
      _canBusOneMessageThread.Start();

      _canBusTwoMessageThread = new Thread(() => ProcessCanBusMessage(CanBusId.CanBus2));
      _canBusTwoMessageThread.Start();
    }
  }
}
