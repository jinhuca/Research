using Modules.ConsoleStateMachine.States;

namespace Modules.ConsoleStateMachine.Transitions;

public interface ITransition<T> where T : IState<T>
{
}