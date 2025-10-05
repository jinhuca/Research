using System;
using Module.Infrastructure.Statuses;
using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class UiStatusEvent : PubSubEvent<(UiStatus, DateTime)> { }
}
