using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Interaction logic for CryoTherapySimpleView.xaml
  /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public partial class CryoTherapySimpleView
  {

    private enum ChartIndex
    {
      TemperatureChart = 0,
      // TipPressureChart,
      // DiaphragmMovementChart,
      // EcgTemperatureChart,
      OcclusionPressureChart
    }

    private WindowsFormsHost TemperatureHost;
    private WindowsFormsHost BloodPressureHost;

    private BackgroundWorker bw;

    /// <summary>
    /// Initializes Cryotherapy components.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public CryoTherapySimpleView()
    {
      InitializeComponent();
      if(bw == null)
      {
        bw = new BackgroundWorker();
        bw.DoWork += bw_DoWork;
        bw.RunWorkerCompleted += bw_RunWorkerCompleted;
      }
    }

    protected override void SubscribeEventHandlers()
    {
      if(_isEventSubscribed)
      {
        return;
      }

      base.SubscribeEventHandlers();

      ObserveTTIButton(VeinIsolationButton);
    }

    // protected override void UnsubscribeEventHandlers()
    // {
    //   base.UnsubscribeEventHandlers();
    //   // _isEventSubscribed = false;
    // }

    /// <summary>
    /// Occurs when the bw_DoWork event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A DoWorkEventArgs that contains the event data.</param>
    private void bw_DoWork(object sender, DoWorkEventArgs e)
    {
      // we  are resting the can two because the first load can take more time than expected 
      CommonViewModel.Current.ResetCanTwoStopWatch();

      BackgroundWorker worker = sender as BackgroundWorker;
      List<Chart> chartList = new List<Chart>();

      // ChartIndex.TemperatureChart
      var chart = InitializeTemperatureGraphic();
      chartList.Add(chart);

      // ChartIndex.OcclusionPressureChart
      chart = InitializeBloodPressureGraphic();
      chartList.Add(chart);

      e.Result = chartList;
    }

    /// <summary>
    /// Occurs when the bw_RunWorkerCompleted event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A RunWorkerCompletedEventArgs that contains the event data.</param>
    private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      Thickness margin;

      try
      {
        // we  are resting the can two because the first load can take more time than expected 
        CommonViewModel.Current.ResetCanTwoStopWatch();
        List<Chart> chartList = (List<Chart>)e.Result;

        //work is done!
        TemperatureHost = new WindowsFormsHost
        {
          Height = 515,
          Width = 1300,
          Visibility = Visibility.Visible
        };

        //Assign the Temperature and Vein isolation series and pressure
        var temperatureChart = (int)ChartIndex.TemperatureChart;
        SetupTemperatureChart(TemperatureHost, chartList[temperatureChart]);
        StackPanelTemperature.Children.Clear();
        StackPanelTemperature.Children.Add(TemperatureHost);

        //Blood pressure
        BloodPressureHost = new WindowsFormsHost
        {
          Height = 515,
          Width = 1300,
          Visibility = Visibility.Visible
        };

        //Blood pressure serie
        var occlusionChart = (int)ChartIndex.OcclusionPressureChart;
        SetupOcclusionPressureGraph(BloodPressureHost, chartList[occlusionChart]);
        StackPanelBloodPressure.Children.Clear();
        StackPanelBloodPressure.Children.Add(BloodPressureHost);

        if (bw != null)
        {
          bw.DoWork -= bw_DoWork;
          bw.RunWorkerCompleted -= bw_RunWorkerCompleted;
          bw = null;
        }
      }
      catch (Exception ex)
      {
        LogException(ex);
        Tuple<long, string, string, string> genericMessage78 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID78, (int)Enumeration.ErrorTypes.GUI);

        Tuple<long, string, string, string> genericMessage77 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID77, (int)Enumeration.ErrorTypes.GUI);

        MessagePopup messagePopup = new MessagePopup(genericMessage77.Item2, MessagePopup.MessageType.WarningMessage, MessagePopup.ButtonType.Ok, genericMessage78.Item2);
        messagePopup.ShowDialog();
      }
    }

    /// <summary>
    /// Occurs when the UserControl_Loaded event is raised.  Start the ECG Reading after the screen has been loaded.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void CryoTherapySimpleView_Loaded(object sender, RoutedEventArgs e)
    {
      cryoTherapyViewModel = DataContext as CryoTherapyViewModel;

      if (cryoTherapyViewModel == null)
        return;

#if Simulator
      CommonViewModel.Current.EcgChannel1And2Reading = 100;
      CommonViewModel.Current.TC1Reading = 25;
#endif
      bw?.RunWorkerAsync();

      base.ControlLoaded();
    }

    /// <summary>
    /// Occurs when the UserControl_Unloaded event is raised.
    /// Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Represents the base class for classes that contain user control event data.</param>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
      base.ControlUnloaded();
    }

    private void CryoTherapySimpleView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (IsLoaded)
      {
        var isVisible = (bool)e.NewValue;
        HandleVisibilityChanged(isVisible);
        if (isVisible)
        {
          TemperatureHost?.InvalidateMeasure();
          BloodPressureHost?.InvalidateMeasure();
          GridTemperatureAblationTime?.InvalidateMeasure();
        }
      }
    }
  }
}
