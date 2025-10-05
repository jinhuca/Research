using System;
using Module.SystemParameters.Models;
using Prism.Events;

namespace Module.SystemParameters.PubSubEvents
{
	public class SystemParamEvent : PubSubEvent<(ISystemParameters, DateTime)> { }
}