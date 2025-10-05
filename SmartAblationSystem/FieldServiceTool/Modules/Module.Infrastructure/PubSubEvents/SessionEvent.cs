using System;
using Prism.Events;

namespace Module.Infrastructure.PubSubEvents
{
	public class SessionStatusEvent : PubSubEvent<(SessionStatus, DateTime)> { }
}
