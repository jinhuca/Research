using Modules.ConsoleStateMachine.States;

namespace Modules.ConsoleStateMachine.Triggers;

public class Trigger<T> : ITrigger<T> where T : IState<T>
{
}