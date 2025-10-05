using DataAccessLayer;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
	/// <summary>
	/// This class is the Action Log View Model
	/// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
	/// </summary>
	public class ActionLogViewModel : BindableBase
    {
        public ICommand ReturnToSettingsCommand { get; private set; }

        /// <summary>
        /// This constructor initializes the Action Log View Model's properties and commands
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public ActionLogViewModel()
        {
            this.ReturnToSettingsCommand = new DelegateCommand<object>(this.OnReturnToSettingsCommand, this.CanReturnToSettingsCommand);
            CommonViewModel.Current.PropertyChanged += Current_PropertyChanged;
        }

       private ObservableCollection<UserAction> _actionLog;
        public ObservableCollection<UserAction> ActionLog
        {
	        get => _actionLog ?? CommonViewModel.Current.ActionLog;
	        set => SetProperty(ref _actionLog, value);
        }

        /// <summary>
        /// Function that returns if the system can invoke the Return To Settings command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReturnToSettingsCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Return To Settings operation when the Return To Settings
        /// command is invoked
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="obj">The command's parameter (not used in this function).</param>
        private void OnReturnToSettingsCommand(object obj)
        {
            ViewsEventArgs viewsEvent = new ViewsEventArgs();
            viewsEvent.ViewName = "BackToSettings";
            CommonViewModel.Current.OnViewchanged(viewsEvent);
        }

        /// <summary>
        /// This function handles the sender's PropertyChanged event
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="sender">The View Model that sent the event.</param>
        /// <param name="e">The property changed arguments.</param>
        private void Current_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CommonViewModel commonviewmodel = sender as CommonViewModel;

            switch (e.PropertyName)
            {
                case "Login":
                    RaisePropertyChanged("Login");
                    break;
            }
        }
    }
}