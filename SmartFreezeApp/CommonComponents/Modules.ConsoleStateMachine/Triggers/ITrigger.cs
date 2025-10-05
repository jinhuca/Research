using Modules.ConsoleStateMachine.States;

namespace Modules.ConsoleStateMachine.Triggers;

public interface ITrigger<T> where T : IState<T>
{
}