using System;
using Modules.ConsoleStateMachine.States;

namespace Modules.ConsoleStateMachine.Interfaces;

public interface IStateMachine<TU, TV>  where TU : MessagePayload where TV : IGPIO
{
  ConsoleState CurrentConsoleState { get; set; }
  IDisposable GetCommunicationStream(IObservable<TU> source);
  IDisposable Output(IObservable<TV> destination);
}