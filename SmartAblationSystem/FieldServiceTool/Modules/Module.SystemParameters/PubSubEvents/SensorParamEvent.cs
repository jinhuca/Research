using System;
using Module.SystemParameters.Interfaces;
using Prism.Events;

namespace Module.SystemParameters.PubSubEvents
{
	public class SensorParamEvent : PubSubEvent<(ISensorParameters, DateTime)> { }
}