using SmartAblationSystem.Helpers;
using SmartAblationSystem.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;
using static SmartAblationSystem.ViewModels.CommonViewModel;

namespace SmartAblationSystem.Views
{
    /// <summary>
    /// Interaction logic for TextEntryPopup.xaml
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public partial class TextEntryPopup : Window
    {
        private object viewModelDataContext = null;
        private TextEntryType entryType;
        private int treatment = 0;

        /// <summary>
        /// Initializes TextEntryPopup components.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="dataContext">An object reprensenting the Data Context.</param>
        /// <param name="entryType">An TextEntryType enum representing the text entry type.</param>
        /// <param name="treatment">An integer representing a treatment.</param>
        public TextEntryPopup(object dataContext, TextEntryType entryType, int treatment = -1)
        {
            InitializeComponent();

            if (dataContext != null)
            {
                viewModelDataContext = dataContext;
            }

            BorderColor.Background = (Brush)Application.Current.Resources["CryterionPopupSystemMessageBrushBackground"];

            this.entryType = entryType;
            this.treatment = treatment;

            if (entryType == TextEntryType.TreatmentNotes)
            {
                TitleLabel.Content = string.Empty;
                if (Models.Languages.GuiFieldTranslation.ContainsKey("NotesTextBlock"))
                {
                    TitleLabel.Content = Models.Languages.GuiFieldTranslation["NotesTextBlock"] + " - " +
                                         CommonViewModel.Current.CurrentAblation.AblationNumber;
                }
                if (!string.IsNullOrWhiteSpace(CommonViewModel.Current?.CurrentAblation?.TreatmentNote))
                {
                    TxtContent.Text = CommonViewModel.Current.CurrentAblation.TreatmentNote;
                   
                }
            }
            else if (entryType == TextEntryType.Diagnosis)
            {
                string diagnosisText = "";
                if (Models.Languages.GuiFieldTranslation.ContainsKey("DiagnosisLabel"))
                {
                    diagnosisText = Models.Languages.GuiFieldTranslation["DiagnosisLabel"];
                }
                TitleLabel.Content = diagnosisText.ToUpper();

                if (!string.IsNullOrWhiteSpace(CommonViewModel.Current?.CurrentProcedure?.Diagnosis))
                {
                    TxtContent.Text = CommonViewModel.Current.CurrentProcedure.Diagnosis;
                }
            }
            else if (entryType == TextEntryType.Outcome)
            {
                string outcomeText = "";
                if (Models.Languages.GuiFieldTranslation.ContainsKey("OutcomeLabel"))
                {
                    outcomeText = Models.Languages.GuiFieldTranslation["OutcomeLabel"];
                }
                TitleLabel.Content = outcomeText.ToUpper();

                if (!string.IsNullOrWhiteSpace(CommonViewModel.Current?.CurrentProcedure?.OutCome))
                {
                    TxtContent.Text = CommonViewModel.Current.CurrentProcedure.OutCome;
                }
            }
            else if (entryType == TextEntryType.ReportTreatmentNotes)
            {
                string noteString = string.Empty;

                if (Models.Languages.GuiFieldTranslation.ContainsKey("NotesTextBlock"))
                {
                    noteString = Models.Languages.GuiFieldTranslation["NotesTextBlock"].ToUpper();
                }

                TitleLabel.Content = noteString + " - " + treatment;
                if (viewModelDataContext != null && ((ReportViewModel)viewModelDataContext).AblationList.Count >= treatment)
                {
                    TxtContent.Text = ((ReportViewModel)viewModelDataContext).AblationList[treatment - 1].Notes;
                    if (TxtContent.Text == "N-A")
                        TxtContent.Text = "";
                }


            }

            TxtContent.Focus();
        }

        /// <summary>
        /// Occurs when the Cancel_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                // TODO
                ex.ToString();
            }
        }

        /// <summary>
        /// Occurs when the OkButton_Click event is raised.
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The component that raised the event.</param>
        /// <param name="e">Contains state information and event data associated with a routed event.</param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (entryType == TextEntryType.TreatmentNotes)
                {
                    CommonViewModel.Current.CurrentAblation.TreatmentNote = TxtContent.Text;
                }
                else if (entryType == TextEntryType.Diagnosis)
                {
                    CommonViewModel.Current.CurrentProcedure.Diagnosis = TxtContent.Text;
                }
                else if (entryType == TextEntryType.Outcome)
                {
                    CommonViewModel.Current.CurrentProcedure.OutCome = TxtContent.Text;
                }
                else if (entryType == TextEntryType.ReportTreatmentNotes)

                {
                    ReportViewModel _viewModelDataContext = ((ReportViewModel)viewModelDataContext);

                    (_viewModelDataContext).AblationList[this.treatment - 1].Notes = TxtContent.Text;

                    (_viewModelDataContext).DataAccess.UpdateAblationNote(this.treatment, (_viewModelDataContext).AblationList[this.treatment - 1].ProcedureId, TxtContent.Text.ToString());


                }

                DialogResult = false;
                this.Close();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }
    }
     
}