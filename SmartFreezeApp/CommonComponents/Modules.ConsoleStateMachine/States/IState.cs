using System;
using System.ComponentModel;
using Modules.Infrastructure.Definitions;

namespace Modules.ConsoleStateMachine.States;

public interface IState<T> : IComparable<T>, IEquatable<T>, INotifyPropertyChanged
{
  string Name { get; }
  ConsoleStateId Id { get; }
  void Enter(T fromState);
  void Exit(T toState);
}