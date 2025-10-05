using Module.Infrastructure.TestInterfaces;
using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class StepEvent : PubSubEvent<IStepModel> { }
}
