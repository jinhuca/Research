using Module.Infrastructure.TestInterfaces;
using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class TestEvent : PubSubEvent<ITestModel> { }
}
