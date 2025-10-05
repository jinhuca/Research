using System;
using Module.SystemParameters.Interfaces;
using Prism.Events;

namespace Module.SystemParameters.PubSubEvents
{
	public class VersionParamEvent : PubSubEvent<(IVersionParameters, DateTime)> { }
}
