using ConsoleCommSimulator.Data;
using Prism.Events;
using System;

namespace ConsoleCommSimulator.Validation
{
  public class RTRThresholdEventUpdater : UpdaterBase
  {

    //private Timer _messageUpdateTimer;
    private IEventAggregator _eventAggregator;

    public RTRThresholdEventUpdater(IEventAggregator eventAggregator) 
    {
      _eventAggregator = eventAggregator;
    }
    public override void PublishUpdate(EventArgs args)
    {
      // Publish the dictionary as an UpdateThresholdEvent
      _eventAggregator.GetEvent<UpdateThresholdEvent>().Publish((UpdateThresholdEventArgs)args);
    }

  }
}
