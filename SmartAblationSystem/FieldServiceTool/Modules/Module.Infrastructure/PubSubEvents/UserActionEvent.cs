using Prism.Events;
using System;
using System.Windows;
using Module.Infrastructure.Constants;

namespace Module.Infrastructure.PubSubEvents
{
	public class UserActionEvent : PubSubEvent<(string, DateTime)> { }

	public class UserCommandEvent : PubSubEvent<(UserCommandMessage, DateTime)> { }
}
