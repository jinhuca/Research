using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Modules.Infrastructure.Definitions;
using static System.Enum;
using static Modules.Infrastructure.Definitions.ConsoleStateId;

namespace Modules.ConsoleStateMachine.States;

public class ConsoleState(ConsoleStateId id) : IState<ConsoleState>
{
  public string Name { get; } = Enum.GetName(id);
  public ConsoleStateId Id { get; } = id;

  public void Enter(ConsoleState fromState)
  {
    switch (Name)
    {
      case nameof(Idle):
        break;
      case nameof(Ready):
        break;
    }
  }

  public void Exit(ConsoleState toState)
  {
    throw new NotImplementedException();
  }

  public static IEnumerable<ConsoleState> GetAllStates() =>
   typeof(ConsoleState)
     .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
     .Select(f => f.GetValue(null))
     .Cast<ConsoleState>();

  public override bool Equals(object obj)
  {
    if (obj is not ConsoleState otherValue)
    {
      return false;
    }

    var typeMatches = GetType() == obj.GetType();
    var valueMatches = Name == otherValue.Name && Id.Equals(otherValue.Id);
    return typeMatches && valueMatches;
  }

  public bool Equals(ConsoleState other) => Name == other?.Name && Id == other?.Id;
  public override int GetHashCode() => HashCode.Combine(Name, Id);
  public int CompareTo(ConsoleState other) => Id.CompareTo(other.Id);
  public event PropertyChangedEventHandler PropertyChanged;

  public static ConsoleState Unknown = new(UnknownStateId);
  public static ConsoleState Idle = new(IdleStateId);
  public static ConsoleState Ready = new(ReadyStateId);
  public static ConsoleState Inflation = new(InflationStateId);
  public static ConsoleState Transition = new(TransitionStateId);
  public static ConsoleState Ablation = new(AblationStateId);
  public static ConsoleState Thawing = new(ThawingStateId);
  public static ConsoleState Exception = new(ExceptionStateId);
}