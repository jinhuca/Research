using Module.Infrastructure.Controls;
using Prism.Events;
using System.Collections.Generic;

namespace Module.Infrastructure.PubSubEvents
{
	public class ErrorListUpdateEvent : PubSubEvent<IList<ErrorMessageExtender>>
  {
  }
}
