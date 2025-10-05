using System.Windows;
using SmartAblationSystem.ViewModels;
using static SmartAblationSystem.ViewModels.CommonViewModel;

namespace SmartAblationSystem.Views
{
  public partial class TextEntryPopupNew : Window
  {
    private object viewModelDataContext = null;
    private TextEntryType entryType;
    private int treatment = 0;

    public TextEntryPopupNew(object dataContext, TextEntryType entryType, int treatment = -1)
    {
      InitializeComponent();
      if(dataContext != null)
      {
        viewModelDataContext = dataContext;
      }
      this.entryType = entryType;
      this.treatment = treatment;
      AdjustWindowPosition();

      if (entryType == TextEntryType.TreatmentNotes)
      {
        TitleTextBlock.Text = "EDIT TREATMENT NOTES";
        SubTitleTextBlock.Text = "NOTES" + " - " + Current.CurrentAblation.AblationNumber;
        if(!string.IsNullOrEmpty(Current?.CurrentAblation?.TreatmentNote))
        {
          TxtContent.Text = Current.CurrentAblation.TreatmentNote;
        }
      }
      else if(entryType == TextEntryType.ReportTreatmentNotes)
      {
        TitleTextBlock.Text = "EDIT TREATMENT NOTES" + " - " + treatment;
        SubTitleTextBlock.Text = "Notes:";
        if(viewModelDataContext != null && ((ReportViewModel)viewModelDataContext).AblationList.Count >= treatment)
        {
          TxtContent.Text = ((ReportViewModel)viewModelDataContext).AblationList[treatment - 1].Notes;
          if(TxtContent.Text == "N-A")
          {
            TxtContent.Text = "";
          }
        }
      }
      else if(entryType == TextEntryType.Diagnosis)
      {
        TitleTextBlock.Text = "EDIT DIAGNOSIS";
        SubTitleTextBlock.Text = "Diagnosis:";
        if(!string.IsNullOrWhiteSpace(Current?.CurrentProcedure?.Diagnosis))
        {
          TxtContent.Text = Current.CurrentProcedure.Diagnosis;
        }
      }
      else if(entryType == TextEntryType.Outcome)
      {
        TitleTextBlock.Text = "EDIT OUTCOME";
        SubTitleTextBlock.Text = "Outcome:";
        if(!string.IsNullOrWhiteSpace(Current?.CurrentProcedure?.OutCome))
        {
          TxtContent.Text = Current.CurrentProcedure.OutCome;
        }
      }
      TxtContent.Focus();
    }
    private void AdjustWindowPosition()
    {
      var screenWidth = SystemParameters.PrimaryScreenWidth;
      var screenHeight = SystemParameters.PrimaryScreenHeight;

      var centerX = (screenWidth - this.Width) / 2;
      var centerY = (screenHeight - this.Height) / 2;

      this.Left = centerX;
      this.Top = centerY - 50;

      // Ensure the window doesn't move off the top of the screen
      if (this.Top < 0)
      {
        this.Top = 0;
      }
    }
    private void No_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = false;
      Close();
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      if(entryType == TextEntryType.TreatmentNotes)
      {
        Current.CurrentAblation.TreatmentNote = TxtContent.Text;
      }
      else if(entryType == TextEntryType.Diagnosis)
      {
        Current.CurrentProcedure.Diagnosis = TxtContent.Text;
      }
      else if(entryType == TextEntryType.Outcome)
      {
        Current.CurrentProcedure.OutCome = TxtContent.Text;
      }
      else if(entryType == TextEntryType.ReportTreatmentNotes)

      {
        ReportViewModel _viewModelDataContext = ((ReportViewModel)viewModelDataContext);

        (_viewModelDataContext).AblationList[treatment - 1].Notes = TxtContent.Text;

        (_viewModelDataContext).DataAccess.UpdateAblationNote(treatment, (_viewModelDataContext).AblationList[treatment - 1].ProcedureId, TxtContent.Text.ToString());


      }

      DialogResult = false;
      Close();
    }
  }
}