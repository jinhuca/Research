using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;
using CustomControls.PubSubEvents;
using MahApps.Metro.Controls;
using Microsoft.Xaml.Behaviors;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;

namespace CustomControls.Behaviors
{
  public class StoryboardSyncBehavior : Behavior<DependencyObject> 
  {
    public string SyncGroupName { get; set; }

    private IEnumerable<Storyboard> _storyboards;

    protected override void OnAttached()
    {
      base.OnAttached();

      if (!(AssociatedObject is FrameworkElement) && !(AssociatedObject is FrameworkContentElement))
        throw new InvalidOperationException("This behavior can only be attached with type FrameworkElement or FrameworkContentElement.");

      var container = (Application.Current as PrismApplication)?.Container;
      var eventAggregator = container?.Resolve<IEventAggregator>();
      eventAggregator?.GetEvent<StoryboardSyncEvent>()?.Subscribe(SynchronizeStoryboard);
    }

    private void InitializeHoldingStoryboardCollection()
    {
      if (AssociatedObject is FrameworkElement frameworkElement)
      {
        _storyboards = frameworkElement.Style?.Triggers
          .SelectMany(t => t.EnterActions.OfType<BeginStoryboard>()
            .Select(s => s.Storyboard))
          .ToList() ?? new List<Storyboard>();
      }
      else
      {
        if (AssociatedObject is FrameworkContentElement associateContentElement) 
        {
          _storyboards = associateContentElement.Style?.Triggers
            .SelectMany(t => t.EnterActions.OfType<BeginStoryboard>()
              .Select(s => s.Storyboard))
            .ToList() ?? new List<Storyboard>();
        }
      }
    } 

    private void SynchronizeStoryboard(string syncGroupName)
    {
      if (string.IsNullOrEmpty(SyncGroupName) || SyncGroupName == syncGroupName)
      {
        this.BeginInvoke(() =>
        {
          InitializeHoldingStoryboardCollection();

          if (AssociatedObject is FrameworkElement frameworkElement)
          {
            foreach (var storyboard in _storyboards)
            {
              StoryboardSeekAlignedToLastTick(storyboard, frameworkElement);
            }
          }
          else if (AssociatedObject is FrameworkContentElement frameworkContentElement)
          {
            foreach (var storyboard in _storyboards)
            {
              StoryboardSeekAlignedToLastTick(storyboard, frameworkContentElement);
            }
          }
        });
      }
    }

    private void StoryboardSeekAlignedToLastTick(Storyboard storyboard, FrameworkElement element) 
    {
      try
      {
        if (storyboard.GetCurrentState(element) != ClockState.Stopped)
          storyboard.SeekAlignedToLastTick(element, new TimeSpan(0, 0, 0, 0), TimeSeekOrigin.BeginTime);
      }
      catch (Exception ex)
      {
        // 
      }
    }

    private void StoryboardSeekAlignedToLastTick(Storyboard storyboard, FrameworkContentElement element) 
    {
      try
      {
        if (storyboard.GetCurrentState(element) != ClockState.Stopped)
          storyboard.SeekAlignedToLastTick(element, new TimeSpan(0, 0, 0, 0), TimeSeekOrigin.BeginTime);
      }
      catch (Exception ex)
      {
        // 
      }
    }
  }
}
