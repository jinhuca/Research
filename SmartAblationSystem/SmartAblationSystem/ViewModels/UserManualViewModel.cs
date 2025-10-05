using Prism.Mvvm;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Xps.Packaging;
using Prism.Commands;

namespace SmartAblationSystem.ViewModels
{
    /// <summary>
    /// This class is the User Manual View Model
    /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
    /// </summary>
    public class UserManualViewModel : BindableBase
    {
        private XpsDocument manualDocument = null;
        private string manualLocation;

        public ICommand ReturnToSettingsCommand { get; private set; }

        /// <summary>
        /// Constructor that initializes User Manual properties
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public UserManualViewModel()
        {
            manualLocation = Directory.GetCurrentDirectory() + @"\UserManual.xps";

            this.ReturnToSettingsCommand = new DelegateCommand<object>(this.OnReturnToSettingsCommand, this.CanReturnToSettingsCommand);
        }

        /// <summary>
        /// This read-only property handles the User Manual Document
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        public FixedDocumentSequence ManualDocument
        {
            get
            {
                if (manualDocument == null)
                {
                    manualDocument = new XpsDocument(manualLocation, FileAccess.Read);
                }
                return manualDocument.GetFixedDocumentSequence();
            }
        }

        /// <summary>
        /// Function that returns if the system can invoke the Return To Settings view command
        /// . Safety classification: No injury or damage to health is possible (IEC 62304 Class A).
        /// </summary>
        /// <param name="arg">The command parameter (not used in this function).</param>
        /// <returns>Boolean value if the system can invoke the command.</returns>
        private bool CanReturnToSettingsCommand(object arg)
        {
            return true;
        }

        /// <summary>
        /// Function/Command that handles the Return to Settings when the Return to Settings view
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
    }
}