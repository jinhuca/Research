using DataAccessLayer;
using Prism.Mvvm;
using SmartAblationSystem.Helpers;
using SmartAblationSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using WarningMessagesManager;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the Warning Messages View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class WarningMessagesViewModel : BindableBase
    {
        private DataAccess dataAccess;

        private bool isClearListVisible = true;

        public ICommand ClearMessagesCommand { get; private set; }

        /// <summary>
        /// This constructor initializes commands and data access
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public WarningMessagesViewModel()
        {
            this.ClearMessagesCommand = new DelegateCommand<object>(this.OnClearMessages, this.CanClearMessages);

            this.dataAccess = CommonViewModel.Current.Data.DataAccess;
            CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
        }

        /// <summary>
        /// This read-only property handles the Warning Messages List
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ObservableCollection<WarningMessage> WarningMessagesList
        {
            get
            {
                return CommonViewModel.Current.WarningMessageManager.WarningMessagesList;
            }
        }

    

        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The parameter's name that has changed.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CommonViewModel commonviewmodel = sender as CommonViewModel;

            switch (e.PropertyName)
            {
                case "WarningMessageManager":
                    RaisePropertyChanged("WarningMessagesList");
                    RaisePropertyChanged("IsClearListVisible");
                    break;
            }
        }

        /// <summary>
        /// Function/Command that displays a popup message (yes/no).  If the answer is Yes, it clears the warning
        /// messages list and calls reset system messages
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">An object passed in parameter when calling the Command.</param>
        private async void OnClearMessages(object obj)
        {
            Tuple<long, string, string, string> genericMessage73 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID73, (int)Enumeration.ErrorTypes.GUI);

            Tuple<long, string, string, string> genericMessage74 = Models.Languages.ErrorsAndCryterionSolutionTranslations((int)Enumeration.GUIMessages.ID74, (int)Enumeration.ErrorTypes.GUI);

            MessagePopup clearWarningPopup = new MessagePopup(genericMessage73.Item2, Views.MessagePopup.MessageType.SystemMessage, MessagePopup.ButtonType.YesNo, genericMessage74.Item2);

            if ((bool)clearWarningPopup.ShowDialog())
            {
                try
                {
                    // In these case we have to reset the system  twice since the CPLD and CMCU keep sending error when we are cleaning the Warnning list 
                    CommonViewModel.Current.ResetSystemAndWarnning();

                    IsClearListVisible = false;
                    await ClearWarningMessagesListWithDelay();
                    CommonViewModel.Current.ResetSystemAndWarnning();
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
                finally
                {
                    IsClearListVisible = true;
                    RaisePropertyChanged("WarningMessagesList");
                }
            }
        }

        /// <summary>
        /// Task that clears the warning messages list
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        private async Task ClearWarningMessagesListWithDelay()
        {
            await Task.Delay(3000);
            CommonViewModel.Current.WarningMessageManager.ClearList();
        }

        /// <summary>
        /// Function that returns if the system can invoke the Clear celMessages command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">An object passed in parameter when calling the Command.</param>
        /// <returns>Boolean that represents if the message list can be cleared.</returns>
        private bool CanClearMessages(object arg)
        {
            return true;
        }

        /// <summary>
        /// This read-only property handles the "Clear List" button's visibility
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public bool IsClearListVisible
        {
            get
            {
                return isClearListVisible;
            }
            set
            {
                isClearListVisible = value;
                RaisePropertyChanged("IsClearListVisible");
            }
        }
    }
}