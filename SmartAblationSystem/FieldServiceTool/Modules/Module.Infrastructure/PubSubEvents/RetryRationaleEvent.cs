using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class RetryRationaleEvent : PubSubEvent<(string, string)> { }
}
