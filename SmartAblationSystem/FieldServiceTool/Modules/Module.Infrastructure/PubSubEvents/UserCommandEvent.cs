using Module.Infrastructure.Constants;
using Prism.Events;
using System;

namespace Module.Infrastructure.PubSubEvents
{
	public class UserCommandEvent : PubSubEvent<(UserCommand, DateTime)> { }
}
