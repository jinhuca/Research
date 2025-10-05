using SmartAblationSystem.Helpers;
using SmartAblationSystem.Models;
using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Shared;
using static LogSystem.LogService;

namespace SmartAblationSystem.Views
{
  /// <summary>
  /// Interaction logic for Report.xaml
  /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
  /// </summary>
  public partial class Report : IDisposable
  {
    private readonly ReportViewModel reportViewModel;

    /// <summary>
    /// Initializes Report components.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public Report()
    {
      InitializeComponent();

      reportViewModel = DataContext as ReportViewModel;
    }

    /// <summary>
    /// Occurs when TreatmentNotes_MouseDown event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">A MouseButtonEventArgs that contains the event data.</param>
    private void TreatmentNotes_MouseDown(object sender, RoutedEventArgs e)
    {
      AblationReport report;
      string previousNote = string.Empty;

      try
      {
        report = (AblationReport)((DataGridRow)((Button)sender).BindingGroup.Owner).DataContext;
        if(report != null && int.TryParse(report.Treatment, out var treatment))
        {
          reportViewModel.CurrentTreatmentNumber = treatment;
          if(treatment - 1 >= 0 && reportViewModel.AblationList.Count > treatment - 1)
          {
            previousNote = reportViewModel.AblationList[treatment - 1].Notes;
          }

          var treatmentNotes = new TextEntryPopupNew(reportViewModel, CommonViewModel.TextEntryType.ReportTreatmentNotes, treatment);
          treatmentNotes.TxtContent.Text = previousNote;
          Opacity = 0.3;
          treatmentNotes.ShowDialog();
          Opacity = 1.0;

          if(treatment - 1 >= 0 && reportViewModel.AblationList.Count > treatment - 1)
          {
            string newNote = reportViewModel.AblationList[treatment - 1].Notes;
            if(!string.Equals(previousNote, newNote) && reportViewModel.MaliciousDataChangeModel.IsMaliciousDataChangeModelActivated)
            {
              reportViewModel.ChangeAblationNote(previousNote, newNote);
            }
          }
        }
      }
      catch(Exception ex)
      {
        LogException(ex);
        var genericMessage79 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID79, (int)Enumeration.ErrorTypes.GUI);
        var genericMessage80 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID80, (int)Enumeration.ErrorTypes.GUI);
        var messagePopup = new MessagePopup(genericMessage79.Item2, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, genericMessage80.Item2);
        messagePopup.ShowDialog();
      }
    }

    /// <summary>
    /// Occurs when the UserControl_Unloaded event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void UserControl_Unloaded(object sender, RoutedEventArgs e)
    {
      SensorReadingMananger.ConnectSensors();
      AblationInformation.IsThereAbltionHistoricalData = true;
      CommonViewModel.Current.AreSensorsInPlayBackMode = false;
    }

    /// <summary>
    /// Occurs when the UserControl_Loaded event is raised.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    /// <param name="sender">The component that raised the event.</param>
    /// <param name="e">Contains state information and event data associated with a routed event.</param>
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
      SensorReadingMananger.DisconnectSensors();
      reportViewModel.ReloadProcedureData();
    }

    /// <summary>
    /// Show BMI tooltip.
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    private void ShowToolTip_Click(object sender, RoutedEventArgs e)
    {
      var toolTip = new ToolTip();

      if(btnBMIInfo.ToolTip != null)
      {

        if(btnBMIInfo.ToolTip is ToolTip)
        {

          toolTip = btnBMIInfo.ToolTip as ToolTip;
          toolTip.IsOpen = true;
        }
      }
    }
    
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
      if(!disposedValue)
      {
        if(disposing)
        {
          // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~Report()
    {
      Dispose(disposing: false);
    }

    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    void IDisposable.Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    private void UIElement_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      try
      {
        var report_ = (AblationReport)((DataGridRow)((Grid)sender).BindingGroup?.Owner)?.DataContext;

        if(report_ != null)
        {
          var treatment_ = int.Parse(report_.Treatment);
          if (report_.AblationSite != AblationSiteEnum.UNKNOWN)
            reportViewModel.ResetAblationSite(report_.AblationSite);

          reportViewModel.CurrentTreatmentNumber = treatment_;
          reportViewModel.DisplayAblationSiteWarning = true;
          var ablationSiteEditWindow_ = new AblationSiteEditWindow(reportViewModel)
          {
            WindowStartupLocation = WindowStartupLocation.CenterScreen
          };
          Opacity = 0.3;
          ablationSiteEditWindow_.ShowDialog();
          Opacity = 1.0;
        }
      }
      catch(Exception ex)
      {
        LogException(ex);
        var genericMessage79 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID79, (int)Enumeration.ErrorTypes.GUI);
        var genericMessage80 = Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID80, (int)Enumeration.ErrorTypes.GUI);
        var messagePopup = new MessagePopup(genericMessage79.Item2, MessagePopup.MessageType.ErrorMessage, MessagePopup.ButtonType.Ok, genericMessage80.Item2);
        messagePopup.ShowDialog();
      }
    }

    private void OnClickDiagnosisButton(object sender, RoutedEventArgs e)
    {
      EditDiagnosis(sender);
    }

    private void OnPreviewMouseUpDiagnosis(object sender, MouseButtonEventArgs e)
    {
      EditDiagnosis(sender);
    }

    private void EditDiagnosis(object sender)
    {
      Opacity = 0.3;
      reportViewModel?.DiagnosisCommand.Execute(sender);
      Opacity = 1.0;
    }

    private void ClickOnOutcomeButton(object sender, RoutedEventArgs e)
    {
      EditOutcome(sender);
    }

    private void OnPreviewMouseUpOutcome(object sender, MouseButtonEventArgs e)
    {
      EditOutcome(sender);
    }

    private void EditOutcome(object sender)
    {
      Opacity = 0.3;
      reportViewModel?.OutcomeCommand.Execute(sender);
      Opacity = 1.0;
    }

    private void Report_OnManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
    {
      e.Handled = true;
    }
  }
}