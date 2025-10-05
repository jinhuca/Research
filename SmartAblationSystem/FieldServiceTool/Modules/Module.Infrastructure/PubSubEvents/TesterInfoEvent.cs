using System;
using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class TesterInfoEvent : PubSubEvent<(string, string, DateTime)> { }
}
