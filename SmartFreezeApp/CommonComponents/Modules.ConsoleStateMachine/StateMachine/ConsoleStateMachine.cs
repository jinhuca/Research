using System;
using System.Collections.Concurrent;
using Modules.ConsoleStateMachine.Interfaces;
using Modules.ConsoleStateMachine.States;

namespace Modules.ConsoleStateMachine.StateMachine;

public class ConsoleStateMachine(ConsoleState ctState) 
  : IStateMachine<MessagePayload, IGPIO>
{
  public ConsoleState CurrentConsoleState { get; set; } = ctState;
  private readonly BlockingCollection<MessagePayload> _blocks = new();

  public IDisposable GetCommunicationStream(IObservable<MessagePayload> source)
  {
    return source.Subscribe(
      data =>
      {
        _blocks.Add(data);

        //switch (CurrentConsoleState.Name)
        //{
        //  case nameof(ConsoleState.Idle):
        //    break;
        //}
      },
      _ =>
      {

      },
      () => {
        });
  }

  public IDisposable Output(IObservable<IGPIO> destination)
  {
    throw new NotImplementedException();
  }
}